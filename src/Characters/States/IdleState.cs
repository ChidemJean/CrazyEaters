using Godot;
using System;
using CrazyEaters.AI.StateMachine;

namespace CrazyEaters.Characters.States
{

   public class IdleState : BaseState
   {
      protected float idleTime = 0.0f;

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
         character.IdleAnimationVariations(idleTime);
      }
   }
}