using Godot;
using System;
using CrazyEaters.AI.StateMachine;

namespace CrazyEaters.Characters.States
{

   public class EatState : BaseState
   {
      public EatState(BaseCharacter baseCharacter) : base(baseCharacter)
      {
         
      }

      public override void OnEnter()
      {
         base.OnEnter();
         character.AnimTree.Set("parameters/Eat/active", true);
      }

      public override void OnExit()
      {
         base.OnExit();
         character.AnimTree.Set("parameters/Eat/active", false);
      }
   }
}