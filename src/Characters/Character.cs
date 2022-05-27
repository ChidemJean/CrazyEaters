namespace CrazyEaters.Characters
{
    using Godot;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CrazyEaters.Managers;

    public class Character : KinematicBody
    {

        [Export] float speed = 6f;
        [Export] float speedFactor = 20f;
        [Export] float gravityScale = 3f;
        [Export] float jumpSpeed = 20f;
        [Export] Vector3 velocity;
        [Export] NodePath animTreePath;
        [Export] NodePath sensorAreaPath;
        [Export] NodePath characterPath;
        [Export] NodePath eyePath;
        [Export] StreamTexture[] eyeTextures;

        Vector3 gravity;
        public Vector3? moveTo = null;
        public bool moving = false;
        float stopDistance = 0;
        GameManager gameManager;
        AnimationTree animationTree;
        AnimationNodeStateMachinePlayback stateMachine;
        Area sensorArea;

        bool isJumping = false;
        bool isFloor = false;
        bool isBlinking = false;

        Spatial character;

        SpatialMaterial eyeMaterial;

        RandomNumberGenerator rng;

        float idleTime = 0.0f;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            gravity = gameManager.gravityVector * gameManager.gravityMagnitude * gravityScale;

            character = GetNode<Spatial>(characterPath);

            sensorArea = GetNode<Area>(sensorAreaPath);
            sensorArea.Connect("body_entered", this, nameof(onSensorBodyEnter));

            animationTree = GetNode<AnimationTree>(animTreePath);
            stateMachine = (AnimationNodeStateMachinePlayback) animationTree.Get("parameters/playback");
            stateMachine.Start("idle");

            eyeMaterial = (SpatialMaterial) GetNode<MeshInstance>(eyePath).Mesh.SurfaceGetMaterial(0);
            
            rng = new RandomNumberGenerator();
            rng.Randomize();
        }

        public async void Blink() {
            isBlinking = true;
            eyeMaterial.AlbedoTexture = eyeTextures[0];
            await Task.Delay(TimeSpan.FromMilliseconds(50));
            eyeMaterial.AlbedoTexture = eyeTextures[1];
            await Task.Delay(TimeSpan.FromMilliseconds(50));
            eyeMaterial.AlbedoTexture = eyeTextures[2];
            await Task.Delay(TimeSpan.FromMilliseconds(50));
            eyeMaterial.AlbedoTexture = eyeTextures[3];
            await Task.Delay(TimeSpan.FromMilliseconds(50));
            eyeMaterial.AlbedoTexture = eyeTextures[4];
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            eyeMaterial.AlbedoTexture = eyeTextures[0];

            rng.Randomize();
            float randomValue = rng.Randf();
            await Task.Delay(TimeSpan.FromMilliseconds(1500 + 3000 * randomValue));
            isBlinking = false;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey) {
                InputEventKey _event = (InputEventKey) @event;
                if (_event.IsPressed() && _event.Scancode == (uint) KeyList.Space) {
                    Jump();
                    return;
                }
                bool onFloor = IsOnFloor();

                if (_event.Scancode == (uint) KeyList.W) {
                    velocity.z = _event.IsPressed() && onFloor ? -1 * speed : 0;
                    MoveAnimation(_event.IsPressed());
                }
                if (_event.Scancode == (uint) KeyList.S) {
                    velocity.z = _event.IsPressed() && onFloor ? 1 * speed : 0;
                    MoveAnimation(_event.IsPressed());
                }
                if (_event.Scancode == (uint) KeyList.A) {
                    velocity.x = _event.IsPressed() && onFloor ? -1 * speed : 0;
                    MoveAnimation(_event.IsPressed());
                }
                if (_event.Scancode == (uint) KeyList.D) {
                    velocity.x = _event.IsPressed() && onFloor ? 1 * speed : 0;
                    MoveAnimation(_event.IsPressed());
                }
            }
        }

        public void MoveAnimation(bool isPressed) {
            if (isPressed && stateMachine.GetCurrentNode() != "walk" && IsOnFloor()) {
                stateMachine.Start("start_walk");
            } else if (velocity.x == 0 && velocity.z == 0) {
                stateMachine.Start("idle");
            }
        }

        public void Jump() {
            if (IsOnFloor()) { 
                isJumping = true;
                stateMachine.Start("jump");
                velocity.y = jumpSpeed;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (stateMachine.GetCurrentNode() == "idle") {
                idleTime += delta;
                IdleAnimationVariations();
            } else {
                idleTime = 0f;
            }

            bool isOnFloor = IsOnFloor();

            if (!isBlinking) {
                Blink();
            }

            if (!isJumping && !isOnFloor && stateMachine.GetCurrentNode() != "jump-loop") {
                stateMachine.Start("jump-loop");
            }

            if (!isOnFloor) {
                velocity += gravity * delta;
            }

            if (isOnFloor && !isFloor) {
                stateMachine.Start("jump_down");
                isJumping = false;
            }

            isFloor = isOnFloor;

            if (velocity.x != 0 || velocity.z != 0) {
                Vector3 targetLook = GlobalTransform.origin + (velocity.Normalized() * 10f);
                targetLook.y = GlobalTransform.origin.y;
                //
                var newTransform = character.GlobalTransform.LookingAt(targetLook, Vector3.Up);
                character.GlobalTransform = character.GlobalTransform.InterpolateWith(newTransform, speed * 2 * delta);
                character.Scale = Vector3.One;
                //
                // character.RotateY(Mathf.LerpAngle(character.Rotation.y, Mathf.Atan2(-targetLook.x, -targetLook.z), speed * delta));
                //
                // character.LookAt(targetLook, Vector3.Up);
            }

            this.MoveAndSlide(velocity, Vector3.Up);
            
        }

        public void IdleAnimationVariations() {
            if (idleTime >= 25) {
                int randIdleAnim = rng.RandiRange(1, 3);
                switch (randIdleAnim) {
                    case 1:
                        stateMachine.Start("idle_foot");
                        break;
                    case 2:
                        stateMachine.Start("idle_head");
                        break;
                    case 3:
                        stateMachine.Start("idle_pose");
                        break;
                }
                idleTime = 0f;
            }
        }

        public void onSensorBodyEnter(Node body) {
            // if (body is StaticBody) {
            //     stateMachine.Start("jump_down");
            //     isJumping = false;
            // }
        }
    }
}