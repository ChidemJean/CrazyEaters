using Godot;
using System;
using CrazyEaters.AI.StateMachine;

namespace CrazyEaters.Characters.States
{

   public class WalkState : BaseState
   {
      public WalkState(BaseCharacter baseCharacter) : base(baseCharacter)
      {
         
      }

      public override void OnEnter()
      {
         base.OnEnter();
         character.AnimTree.Set("parameters/Walk/blend_amount", 1);
         character.AnimTree.Set("parameters/State/current", 0);
      }

      public override void OnExit()
      {
         base.OnExit();
         character.AnimTree.Set("parameters/Walk/blend_amount", 0);
      }
   }
}