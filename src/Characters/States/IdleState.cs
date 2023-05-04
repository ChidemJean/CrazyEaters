using Godot;
using System;
using CrazyEaters.AI.StateMachine;

namespace CrazyEaters.Characters.States
{

   public class IdleState : BaseState
   {
      protected float idleTime = 0.0f;
      protected double radius = 200f;

      public IdleState(BaseCharacter baseCharacter) : base(baseCharacter)
      {

      }

      public override void OnEnter()
      {
         base.OnEnter();
         character.AnimTree.Set("parameters/Walk/blend_amount", 0);
         character.AnimTree.Set("parameters/State/current", 0);
      }

      public override void OnExit()
      {
         base.OnExit();
         idleTime = 0f;
      }

      public override void Tick(float delta)
      {
         base.Tick(delta);
         idleTime += delta;
         idleTime = character.IdleAnimationVariations(idleTime);
         if (idleTime > 5) {
            bool chance = GD.Randf() > .3f;
            if (chance) {
               character.targetIALocation = character.GlobalTransform.origin + new Vector3((float) GD.RandRange(-radius, radius), 0, (float) GD.RandRange(-radius, radius));
               character.UpdatePath();
            }
         }
      }
   }
}