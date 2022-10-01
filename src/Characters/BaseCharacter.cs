using Godot;
using System;
using CrazyEaters.Managers;
using CrazyEaters.Entities;
using CrazyEaters.Characters.States;
using CrazyEaters.AI.StateMachine;
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
      [Export] protected NodePath sensorMouthPath;
      [Export] protected NodePath worldPath;
      [Export] protected NodePath viewFieldPath;

      protected CrazyEaters.Sandbox.World world;
      protected Vector3 gravity;
      protected Vector3? moveTo = null;
      protected bool moving = false;
      protected float stopDistance = 0;
      protected GameManager gameManager;
      protected AnimationTree animationTree;
      protected AnimationPlayer animationPlayer;
      public AnimationTree AnimTree => animationTree;
      protected AnimationNodeStateMachinePlayback stateMachine;
      protected Area sensorArea;
      protected Area sensorMouth;
      protected Area viewField;
      protected float _speed;
      protected Vector3 moveDir = Vector3.Zero;
      protected Vector3 OriginalScale = Vector3.One;
      protected bool isJumping = false;
      protected bool isFloor = false;
      protected bool isBlinking = false;
      protected bool canWalk = true;
      protected RandomNumberGenerator rng;

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
      private StateMachine _ai_stateMachine;

      public StateMachine ai_StateMachine => _ai_stateMachine;

      public bool eating = false;
      public bool openMouth = false;

      public override void _Ready()
      {
         gameManager = GetNode<GameManager>("/root/GameManager");
         gravity = gameManager.gravityVector * gameManager.gravityMagnitude * gravityScale;
         world = GetNode<CrazyEaters.Sandbox.World>(worldPath);
         sensorArea = GetNode<Area>(sensorAreaPath);
         sensorMouth = GetNode<Area>(sensorMouthPath);
         viewField = GetNode<Area>(viewFieldPath);

         animationTree = GetNode<AnimationTree>(animTreePath);
         stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
         animationPlayer = animationTree.GetNode<AnimationPlayer>(animationTree.AnimPlayer);

         rng = new RandomNumberGenerator();
         rng.Randomize();
         // IA
         navigation = world.navigation;
         navmesh = world.navmesh;
         navigationAgent = GetNode<NavigationAgent>(navigationAgentPath);
         ig = GetNode<ImmediateGeometry>(igPath);

         navmesh.Connect("bake_finished", this, nameof(OnNavmeshChanged));
         sensorArea.Connect("body_entered", this, nameof(OnBodyEnteredMouth));
         viewField.Connect("body_entered", this, nameof(OnBodyEnteredViewField));
         // navigationAgent.SetTargetLocation(targetIALocation);

         _ai_stateMachine = new StateMachine();
         _ai_stateMachine.AddAnyTransition(new IdleState(this), () => {
            return moveDir.x == 0 && moveDir.z == 0 && !openMouth;
         });
         _ai_stateMachine.AddAnyTransition(new WalkState(this), () => {
            return moveDir != Vector3.Zero && canWalk;
         });
         _ai_stateMachine.AddAnyTransition(new OpenMouthState(this), () => {
            return openMouth;
         });

         _speed = speed;

         OriginalScale = Scale;
      }

      public abstract void OnNavmeshChanged();
      public abstract void IdleAnimationVariations(float idleTime);
      public async void OnBodyEnteredMouth(Node body)
      {
         if (body is Food)
         {
            eating = true;
            AnimTree.Set("parameters/Eat/active", true);
            await Task.Delay(TimeSpan.FromSeconds(1.93f));
            openMouth = false;
         }
      }
      public void OnBodyEnteredViewField(Node body)
      {
         if (body is Food)
         {
            openMouth = true;
         }
      }

      public void MoveAnimation(bool moving)
      {
         // if (moving && stateMachine.GetCurrentNode() != "walk" && stateMachine.GetCurrentNode() != "start_walk" && IsOnFloor() && canWalk)
         // {
         //    stateMachine.Start("start_walk");
         // }
         // else if (moveDir.x == 0 && moveDir.z == 0 && stateMachine.GetCurrentNode() != "eat" && !stateMachine.GetCurrentNode().Contains("idle"))
         // {
         //    stateMachine.Start("idle");
         // }
      }
      public void Jump()
      {
         if (IsOnFloor())
         {
            isJumping = true;
            // stateMachine.Start("jump");
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
         
         if (ai_StateMachine != null) {
            ai_StateMachine.Tick(delta);
         }

         // if (!isJumping && !isOnFloor && stateMachine.GetCurrentNode() != "jump-loop")
         // {
            // stateMachine.Start("jump-loop");
         // }

         if (!isOnFloor)
         {
            velocity += gravity * delta;
         }
         else
         {
            UpdateAIMovement();
            velocity.x = moveDir.x * _speed;
            velocity.z = moveDir.z * _speed;
         }

         if (isOnFloor && !isFloor)
         {
            // stateMachine.Start("jump_down");
            OnJumpDownAnimationEnd();
            isJumping = false;
         }
         isFloor = isOnFloor;

         MoveAnimation(moveDir != Vector3.Zero);

         if (pathIA != null && pathIA.Length > 0)
         {
            Vector3 look = (pathIA[pathNodeIA]);
            look.y = GlobalTransform.origin.y;
            var newTransform = GlobalTransform.LookingAt(look, Vector3.Up);
            GlobalTransform = GlobalTransform.InterpolateWith(newTransform, speed * 1.42f * delta);
         }
         velocity = this.MoveAndSlideWithSnap(velocity, isJumping ? Vector3.Zero : Vector3.Down, Vector3.Up, true, 4, Mathf.Deg2Rad(52));

         DebugPath();
      }

      public async void OnJumpDownAnimationEnd()
      {
         await Task.Delay(TimeSpan.FromSeconds(.57f));
         canWalk = true;
      }

      public void OnEatAnimationEnd()
      {
         eating = false;
         GD.Print("eating end");
      }
   }
}