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
   using CrazyEaters.Controllers;
   using CrazyEaters.DependencyInjection;

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

      public override void _Ready()
      {
			base._Ready();
         this.ResolveDependencies();

         eyeMaterial = (SpatialMaterial)GetNode<MeshInstance>(eyePath).Mesh.SurfaceGetMaterial(0);
         scene = ((HabitatScene) sSwitcher.currentScene);

         // Debug
         labelVel = GetNode<Label>(labelVelPath);

         gm.StartListening(GameEvent.UpdateCharacterStatus, OnUpdateStatus);
      }

      public void OnUpdateStatus(object param) 
      {}

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
      }

      public override void OnNavmeshChanged()
      {
         
      }

      public override void IdleAnimationVariations(float idleTime)
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
            idleTime = 0f;
         }
      }
   }
}