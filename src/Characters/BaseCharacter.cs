using Godot;
using System;
using CrazyEaters.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrazyEaters.Characters
{
   public abstract class BaseCharacter : KinematicBody
   {
      [Export] protected float speed = 6f;
      [Export] protected float speedFactor = 20f;
      [Export] protected float gravityScale = 3f;
      [Export] protected float jumpSpeed = 20f;
      [Export] protected Vector3 velocity;
      [Export] protected NodePath animTreePath;
      [Export] protected NodePath sensorAreaPath;
      [Export] protected NodePath worldPath;

      protected CrazyEaters.Sandbox.World world;
      protected Vector3 gravity;
      protected Vector3? moveTo = null;
      protected bool moving = false;
      protected float stopDistance = 0;
      protected GameManager gameManager;
      protected AnimationTree animationTree;
      protected AnimationPlayer animationPlayer;
      protected AnimationNodeStateMachinePlayback stateMachine;
      protected Area sensorArea;
      protected float _speed;
      protected Vector3 moveDir = Vector3.Zero;
      protected Vector3 OriginalScale = Vector3.One;
      protected bool isJumping = false;
      protected bool isFloor = false;
      protected bool isBlinking = false;
      protected bool canWalk = true;
      protected RandomNumberGenerator rng;
 
      protected float idleTime = 0.0f;
      // IA
      [Export]
      public NodePath navigationAgentPath;
      [Export]
      public NodePath igPath;
      public NavigationAgent navigationAgent;
      public Navigation navigation;
      public NavigationMeshInstance navmesh;
      public Vector3? targetIALocation = null;
      public int pathNodeIA = 0;
      public Vector3[] pathIA;

      public ImmediateGeometry ig;

      public override void _Ready()
      {
         gameManager = GetNode<GameManager>("/root/GameManager");
         gravity = gameManager.gravityVector * gameManager.gravityMagnitude * gravityScale;
         world = GetNode<CrazyEaters.Sandbox.World>(worldPath);
         sensorArea = GetNode<Area>(sensorAreaPath);

         animationTree = GetNode<AnimationTree>(animTreePath);
         stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
         stateMachine.Start("idle");
         animationPlayer = animationTree.GetNode<AnimationPlayer>(animationTree.AnimPlayer);

         rng = new RandomNumberGenerator();
         rng.Randomize();
         // IA
         navigation = world.navigation;
         navmesh = world.navmesh;
         navigationAgent = GetNode<NavigationAgent>(navigationAgentPath);
         ig = GetNode<ImmediateGeometry>(igPath);

         navmesh.Connect("bake_finished", this, nameof(OnNavmeshChanged));
         // navigationAgent.SetTargetLocation(targetIALocation);

         _speed = speed;

         OriginalScale = Scale;
      }

      public abstract void OnNavmeshChanged();
      public abstract void IdleAnimationVariations();

      public void MoveAnimation(bool isPressed)
      {
         if (isPressed && stateMachine.GetCurrentNode() != "walk" && IsOnFloor())
         {
            stateMachine.Start("start_walk");
         }
         else if (moveDir.x == 0 && moveDir.z == 0)
         {
            stateMachine.Start("idle");
         }
      }
      public void Jump()
      {
         if (IsOnFloor())
         {
            isJumping = true;
            stateMachine.Start("jump");
            velocity.y = jumpSpeed;
            canWalk = false;
         }
      }

      public void DebugPath()
      {
         if (pathIA != null && pathIA.Length > 0)
         {
            ig.SetAsToplevel(true);
            // ig.GlobalTransform = new Transform();
            ig.Clear();
            ig.Begin(Mesh.PrimitiveType.Lines);
            foreach (int i in GD.Range(pathIA.Length - 1))
            {
               ig.AddVertex(pathIA[i]);
               ig.AddVertex(pathIA[i + 1]);
            }
            if (pathIA.Length == 0)
            {
               ig.End();
               return;
            }
            List<int> _list = new List<int>();
            _list.AddRange(GD.Range(pathIA.Length));

            if (_list.Contains(pathNodeIA))
            {
               ig.AddVertex(pathIA[pathNodeIA]);
               ig.AddVertex(pathIA[pathNodeIA] + new Vector3(0, 3, 0));
            }
            ig.End();
         }
         ig.Translation = Vector3.Zero;
      }

      public void UpdatePath()
      {
         pathNodeIA = 0;
         pathIA = navigation.GetSimplePath(GlobalTransform.origin, (Vector3)targetIALocation);
      }

      public void UpdateAIMovement()
      {
         // IA MOVEMENT
         if (pathIA != null && pathIA.Length > 0 && pathNodeIA < pathIA.Length)
         {
            Vector3 dir = (pathIA[pathNodeIA] - GlobalTransform.origin).Normalized();
            dir.y = 0;

            if (dir.Length() < .25f)
            {
               if (pathNodeIA == pathIA.Length - 1)
               {
                  dir = Vector3.Zero;
               }
               else
               {
                  pathNodeIA += 1;
               }
            }

            moveDir = dir;
            return;
         }
         if (targetIALocation != null)
         {
            UpdatePath();
         }
      }

      public override void _PhysicsProcess(float delta)
      {
         bool isOnFloor = IsOnFloor();

         if (canWalk && stateMachine.GetCurrentNode() != "walk" && stateMachine.GetCurrentNode() != "start_walk" && moveDir != Vector3.Zero && isOnFloor)
         {
            stateMachine.Start("start_walk");
         }

         if (stateMachine.GetCurrentNode() == "idle")
         {
            idleTime += delta;
            IdleAnimationVariations();
         }
         else
         {
            idleTime = 0f;
         }

         if (!isJumping && !isOnFloor && stateMachine.GetCurrentNode() != "jump-loop")
         {
            stateMachine.Start("jump-loop");
         }

         Vector3 correction = Vector3.Zero;

         if (!isOnFloor)
         {
            velocity += gravity * delta;
         }
         else
         {
            // correction = Vector3.Up - GetFloorNormal();
            // correction *= gravity;
            // correction *= -1;

            UpdateAIMovement();
            velocity.x = moveDir.x * _speed;
            velocity.z = moveDir.z * _speed;
         }

         if (isOnFloor && !isFloor)
         {
            stateMachine.Start("jump_down");
            OnJumpDownAnimationEnd();
            isJumping = false;
         }

         isFloor = isOnFloor;

         // Vector3 localVelocity = velocity.Rotated(Vector3.Up, Rotation.y);
         // Vector3 targetLook = (GlobalTransform.origin + (localVelocity.Normalized()));
         // targetLook.y = GlobalTransform.origin.y;

         // if (controllerMode == CharacterControllerMode.PLAYER && (localVelocity.x != 0 || localVelocity.z != 0) && moveDir.z != 1) {
         //     //
         //     var newTransform = GlobalTransform.LookingAt(targetLook, Vector3.Up);
         //     GlobalTransform = GlobalTransform.InterpolateWith(newTransform, speed * 1.2f * delta);
         //     Scale = OriginalScale;
         //     //
         //     // character.RotateY(Mathf.LerpAngle(character.Rotation.y, Mathf.Atan2(-targetLook.x, -targetLook.z), speed * delta));
         //     //
         //     // character.LookAt(targetLook, Vector3.Up);
         // } else {

         // }

         if (moveDir != Vector3.Zero)
         {
            MoveAnimation(false);
         }
         else
         {
            MoveAnimation(true);
         }

         if (pathIA != null && pathIA.Length > 0)
         {
            Vector3 look = (pathIA[pathNodeIA]);
            look.y = GlobalTransform.origin.y;
            // character.LookAt(look, Vector3.Up);
            var newTransform = GlobalTransform.LookingAt(look, Vector3.Up);
            GlobalTransform = GlobalTransform.InterpolateWith(newTransform, speed * 1.42f * delta);
         }
         velocity = this.MoveAndSlideWithSnap(velocity + correction, isJumping ? Vector3.Zero : Vector3.Down, Vector3.Up, true, 4, Mathf.Deg2Rad(52));

         DebugPath();
      }

      public async void OnJumpDownAnimationEnd()
      {
         // Vector3 tempMoveDir = moveDir;
         // moveDir = Vector3.Zero;
         await Task.Delay(TimeSpan.FromSeconds(.57f));
         // moveDir = tempMoveDir;
         canWalk = true;
      }
   }
}