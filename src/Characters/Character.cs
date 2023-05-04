namespace CrazyEaters.Characters
{
   using Godot;
   using System;
   using System.Collections.Generic;
   using System.Threading;
   using System.Threading.Tasks;
   using CrazyEaters.Managers;
   using CrazyEaters.Sandbox;
   using CrazyEaters.Resources;
   using CrazyEaters.Characters.States;
   using CrazyEaters.Controllers;
   using CrazyEaters.DependencyInjection;
   using CrazyEaters.Entities;

   public class Character : BaseCharacter
   {
      public enum CharacterControllerMode
      {
         PLAYER,
         IA
      };

      [Export] NodePath eyePath;
      [Export] StreamTexture[] eyeTextures;
      [Export] CharacterControllerMode controllerMode = CharacterControllerMode.IA;
      [Export] StatusesCharacter statusesResource;
      [Export] PackedScene tombstone;

      List<CrazyEaters.Save.StatusCharacter> statuses;

      Spatial character;

      SpatialMaterial eyeMaterial;

      // Debug
      [Export]
      public NodePath labelVelPath;
      public Label labelVel;
      [Inject] private SceneSwitcher sSwitcher = null;
      [Inject] GameManager gm;
      private HabitatScene scene = null;

      public bool isDead = false;

      public override void _Ready()
      {
         base._Ready();
         this.ResolveDependencies();

         eyeMaterial = (SpatialMaterial)GetNode<MeshInstance>(eyePath).Mesh.SurfaceGetMaterial(0);
         scene = ((HabitatScene)sSwitcher.currentScene);

         // Debug
         labelVel = GetNode<Label>(labelVelPath);

         gm.StartListening(GameEvent.UpdateCharacterStatus, OnUpdateStatus);

         ai_StateMachine.AddAnyTransition(new IdleState(this), () =>
         {
            return moveDir.x == 0 && moveDir.z == 0 && !openMouth;
         });
         ai_StateMachine.AddAnyTransition(new WalkState(this), () =>
         {
            return moveDir != Vector3.Zero && canWalk;
         });
         ai_StateMachine.AddAnyTransition(new OpenMouthState(this), () =>
         {
            return openMouth;
         });
         WalkState walkState = ai_StateMachine.GetState<WalkState>();
      }

      public void OnUpdateStatus(object param)
      {
         CharacterStatusEventData status = (CharacterStatusEventData)param;
         if (status.name == "health" && statusesResource.statuses["health"].curValue == 0)
         {
            Die();
         }
      }

      public async void Blink()
      {
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
         await Task.Delay(TimeSpan.FromMilliseconds(1500 + 1000 * randomValue));
         isBlinking = false;
      }

      public override void _Input(InputEvent @event)
      {
         if (controllerMode == CharacterControllerMode.PLAYER)
         {
            if (@event is InputEventKey)
            {
               InputEventKey _event = (InputEventKey)@event;
               if (_event.IsPressed() && _event.Scancode == (uint)KeyList.Space)
               {
                  Jump();
                  return;
               }
               bool onFloor = IsOnFloor();

               if (_event.Scancode == (uint)KeyList.Shift)
               {
                  if (_event.IsPressed())
                  {
                     _speed = speed * 1.75f;
                     animationPlayer.PlaybackSpeed *= 6;
                  }
                  else
                  {
                     _speed = speed;
                     animationPlayer.PlaybackSpeed /= 6;
                  }
               }

               if (_event.Scancode == (uint)KeyList.W)
               {
                  moveDir.z = _event.IsPressed() && onFloor && canWalk ? -1 : 0;
                  // MoveAnimation(_event.IsPressed());
               }
               if (_event.Scancode == (uint)KeyList.S)
               {
                  moveDir.z = _event.IsPressed() && onFloor && canWalk ? 1 : 0;
                  // MoveAnimation(_event.IsPressed());
               }
               if (_event.Scancode == (uint)KeyList.A)
               {
                  moveDir.x = _event.IsPressed() && onFloor && canWalk ? -1 : 0;
                  // MoveAnimation(_event.IsPressed());
               }
               if (_event.Scancode == (uint)KeyList.D)
               {
                  moveDir.x = _event.IsPressed() && onFloor && canWalk ? 1 : 0;
                  // MoveAnimation(_event.IsPressed());
               }
            }
            if (@event is InputEventMouseButton)
            {
               InputEventMouseButton _event = (InputEventMouseButton)@event;
               if (!_event.IsPressed() && _event.ButtonIndex == (uint)ButtonList.Left && !scene.inEditMode)
               {
                  Vector2 mousePos = _event.Position * gameManager.hud.currentScale;
                  PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
                  Vector3 rayOrigin = GetViewport().GetCamera().ProjectRayOrigin(mousePos);
                  Vector3 rayEnd = rayOrigin + GetViewport().GetCamera().ProjectRayNormal(mousePos) * 2000;
                  Godot.Collections.Dictionary rayCol = spaceState.IntersectRay(rayOrigin, rayEnd, null);
                  if (rayCol != null && rayCol.Count > 0)
                  {
                     targetIALocation = (Vector3)rayCol["position"];
                     UpdatePath();
                  }
               }
            }
         }
      }

      public override void _PhysicsProcess(float delta)
      {
         base._PhysicsProcess(delta);
         if (!isBlinking)
         {
            Blink();
         }
         if (!isDead)
         {
            foreach (KeyValuePair<string, StatusCharacter> item in statusesResource.statuses)
            {
               StatusCharacter status = item.Value;
               if (status.seconds > 0)
               {
                  status.curSeconds += delta;
               }
               if (status.secondsForDamage > 0 && status.curValue == 0)
               {
                  status.curSecondsForDamage += delta;
               }
               if (status.curSeconds > status.seconds)
               {
                  status.curValue = Mathf.Clamp(status.curValue + status.variation, 0, status.max);
                  gm.TriggerEvent(GameEvent.UpdateCharacterStatus, new CharacterStatusEventData(status.key, status.variation));
                  status.curSeconds = 0;
               }
               if (status.damage != 0 && status.curSecondsForDamage > status.secondsForDamage)
               {
                  var healthStatus = statusesResource.statuses["health"];
                  healthStatus.curValue = Mathf.Clamp(healthStatus.curValue + status.damage, 0, healthStatus.max);
                  gm.TriggerEvent(GameEvent.UpdateCharacterStatus, new CharacterStatusEventData("health", status.damage));
                  status.curSecondsForDamage = 0;
               }
            }
         }
      }

      public void UpdateStatus(string statusKey, int variation)
      {
         StatusCharacter status = null;
         statusesResource.statuses.TryGetValue(statusKey, out status);
         if (status == null) return;
         status.curValue = Mathf.Clamp(status.curValue + variation, 0, status.max);
         gm.TriggerEvent(GameEvent.UpdateCharacterStatus, new CharacterStatusEventData(statusKey, variation));
      }

      public override void OnNavmeshChanged()
      {

      }

      public override float IdleAnimationVariations(float idleTime)
      {
         if (idleTime >= 25)
         {
            int randIdleAnim = rng.RandiRange(1, 3);

            switch (randIdleAnim)
            {
               case 1:
                  AnimTree.Set("parameters/IdleFoot/active", true);
                  break;
               case 2:
                  AnimTree.Set("parameters/IdleHead/active", true);
                  break;
               case 3:
                  AnimTree.Set("parameters/IdlePose/active", true);
                  break;
            }
            return 0f;
         }
         return idleTime;
      }

      public override void _ExitTree()
      {
         gm.StopListening(GameEvent.UpdateCharacterStatus, OnUpdateStatus);
      }

      public override void Eat(Food food)
      {
         base.Eat(food);
         UpdateStatus("hungry", food.GetCalories());
      }

      public override void TakeDamage(int damage)
      {
         
      }

      public override void Die()
      {
         isDead = true;
         SpawnTombstone();
         QueueFree();
      }

      public void SpawnTombstone()
      {
         if (tombstone != null)
         {
            var tombNode = tombstone.Instance<Tombstone>();
            gm.currentMainNode3D?.AddChild(tombNode);
            tombNode.GlobalTranslation = new Vector3(this.GlobalTransform.origin + new Vector3(0, 10f, 0));
         }
      }
   }
}