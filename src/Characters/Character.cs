namespace CrazyEaters.Characters
{
    using Godot;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CrazyEaters.Managers;

    public class Character : KinematicBody
    {
        public enum CharacterControllerMode {
            PLAYER,
            IA
        };

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
        [Export] CharacterControllerMode controllerMode = CharacterControllerMode.IA;

        Vector3 gravity;
        public Vector3? moveTo = null;
        public bool moving = false;
        float stopDistance = 0;
        GameManager gameManager;
        AnimationTree animationTree;
        AnimationPlayer animationPlayer;
        AnimationNodeStateMachinePlayback stateMachine;
        Area sensorArea;
        float _speed;
        Vector3 moveDir = Vector3.Zero;
        Vector3 OriginalScale = Vector3.One;

        bool isJumping = false;
        bool isFloor = false;
        bool isBlinking = false;
        bool canWalk = true;

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

            animationTree = GetNode<AnimationTree>(animTreePath);
            stateMachine = (AnimationNodeStateMachinePlayback) animationTree.Get("parameters/playback");
            stateMachine.Start("idle");
            animationPlayer = animationTree.GetNode<AnimationPlayer>(animationTree.AnimPlayer);
            
            eyeMaterial = (SpatialMaterial) GetNode<MeshInstance>(eyePath).Mesh.SurfaceGetMaterial(0);
            
            rng = new RandomNumberGenerator();
            rng.Randomize();

            _speed = speed;

            OriginalScale = Scale;
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
            if (controllerMode == CharacterControllerMode.PLAYER) {
                if (@event is InputEventKey) {
                    InputEventKey _event = (InputEventKey) @event;
                    if (_event.IsPressed() && _event.Scancode == (uint) KeyList.Space) {
                        Jump();
                        return;
                    }
                    bool onFloor = IsOnFloor();

                    if (_event.Scancode == (uint) KeyList.Shift) {
                        if (_event.IsPressed()) {
                            _speed = speed * 1.75f;
                            animationPlayer.PlaybackSpeed *= 6;
                        } else {
                            _speed = speed;
                            animationPlayer.PlaybackSpeed /= 6;
                        }
                    }

                    if (_event.Scancode == (uint) KeyList.W) {
                        moveDir.z = _event.IsPressed() && onFloor && canWalk ? -1 : 0;
                        MoveAnimation(_event.IsPressed());
                    }
                    if (_event.Scancode == (uint) KeyList.S) {
                        moveDir.z = _event.IsPressed() && onFloor && canWalk ? 1 : 0;
                        MoveAnimation(_event.IsPressed());
                    }
                    if (_event.Scancode == (uint) KeyList.A) {
                        moveDir.x = _event.IsPressed() && onFloor && canWalk ? -1 : 0;
                        MoveAnimation(_event.IsPressed());
                    }
                    if (_event.Scancode == (uint) KeyList.D) {
                        moveDir.x = _event.IsPressed() && onFloor && canWalk ? 1 : 0;
                        MoveAnimation(_event.IsPressed());
                    }
                }
            }
        }

        public void MoveAnimation(bool isPressed) {
            if (isPressed && stateMachine.GetCurrentNode() != "walk" && IsOnFloor()) {
                stateMachine.Start("start_walk");
            } else if (moveDir.x == 0 && moveDir.z == 0) {
                stateMachine.Start("idle");
            }
        }

        public void Jump() {
            if (IsOnFloor()) { 
                isJumping = true;
                stateMachine.Start("jump");
                velocity.y = jumpSpeed;
                canWalk = false;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            bool isOnFloor = IsOnFloor();

            if (canWalk && stateMachine.GetCurrentNode() != "walk" && stateMachine.GetCurrentNode() != "start_walk" && moveDir != Vector3.Zero && isOnFloor) {
                stateMachine.Start("start_walk");
            }

            if (stateMachine.GetCurrentNode() == "idle") {
                idleTime += delta;
                IdleAnimationVariations();
            } else {
                idleTime = 0f;
            }

            if (!isBlinking) {
                Blink();
            }

            if (!isJumping && !isOnFloor && stateMachine.GetCurrentNode() != "jump-loop") {
                stateMachine.Start("jump-loop");
            }

            if (!isOnFloor) {
                velocity += gravity * delta;
            } else {
                velocity.x = moveDir.x * _speed;
                velocity.z = moveDir.z * _speed;
            }

            if (isOnFloor && !isFloor) {
                stateMachine.Start("jump_down");
                OnJumpDownAnimationEnd();
                isJumping = false;
            }

            isFloor = isOnFloor;

            Vector3 localVelocity = velocity.Rotated(Vector3.Up, Rotation.y);

            if ((localVelocity.x != 0 || localVelocity.z != 0) && moveDir.z != 1) {
                Vector3 targetLook = GlobalTransform.origin + (localVelocity.Normalized() * 10f);
                targetLook.y = GlobalTransform.origin.y;
                //
                var newTransform = GlobalTransform.LookingAt(targetLook, Vector3.Up);
                GlobalTransform = GlobalTransform.InterpolateWith(newTransform, speed * 1.2f * delta);
                Scale = OriginalScale;
                //
                // character.RotateY(Mathf.LerpAngle(character.Rotation.y, Mathf.Atan2(-targetLook.x, -targetLook.z), speed * delta));
                //
                // character.LookAt(targetLook, Vector3.Up);
            }

            this.MoveAndSlide(localVelocity, Vector3.Up);
            
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

        public async void OnJumpDownAnimationEnd() {
            // Vector3 tempMoveDir = moveDir;
            // moveDir = Vector3.Zero;
            await Task.Delay(TimeSpan.FromSeconds(.57f));
            // moveDir = tempMoveDir;
            canWalk = true;
        }
    }
}