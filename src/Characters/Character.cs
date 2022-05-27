namespace CrazyEaters.Characters
{
    using Godot;
    using System;
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

        Vector3 gravity;
        public Vector3? moveTo = null;
        public bool moving = false;
        float stopDistance = 0;
        GameManager gameManager;
        AnimationTree animationTree;
        AnimationNodeStateMachinePlayback stateMachine;
        Area sensorArea;

        bool isJumping = false;

        Spatial character;

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
                stateMachine.Start("walk");
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
            bool isOnFloor = IsOnFloor();
            if (!isJumping && !isOnFloor && stateMachine.GetCurrentNode() != "jump-loop") {
                stateMachine.Travel("jump-loop");
            }

            if (!isOnFloor) {
                velocity += gravity * delta;
            }

            if (velocity.x != 0 || velocity.z != 0) {
                Vector3 targetLook = GlobalTransform.origin + (velocity.Normalized() * 10f);
                targetLook.y = GlobalTransform.origin.y;
                //
                var newTransform = character.GlobalTransform.LookingAt(targetLook, Vector3.Up);
                character.GlobalTransform = character.GlobalTransform.InterpolateWith(newTransform, speed * delta);
                character.Scale = Vector3.One;
                // character.LookAt(targetLook, Vector3.Up);
            }

            this.MoveAndSlide(velocity, Vector3.Up);
            
        }

        public void onSensorBodyEnter(Node body) {
            if (body is StaticBody) {
                stateMachine.Start("jump_down");
                isJumping = false;
            }
        }
    }
}