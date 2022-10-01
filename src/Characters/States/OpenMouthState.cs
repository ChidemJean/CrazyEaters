using Godot;
using System;
using System.Threading.Tasks;
using CrazyEaters.AI.StateMachine;

namespace CrazyEaters.Characters.States
{

   public class OpenMouthState : BaseState
   {
      public OpenMouthState(BaseCharacter baseCharacter) : base(baseCharacter)
      {

      }

      public override void OnEnter()
      {
         base.OnEnter();
         character.AnimTree.Set("parameters/OpenMouth/add_amount", 1);
         Timeout();
      }

      public override void OnExit()
      {
         base.OnExit();
         character.AnimTree.Set("parameters/OpenMouth/add_amount", 0);
      }

      private async void Timeout()
      {
         await Task.Delay(TimeSpan.FromSeconds(4));
         if (character.openMouth) CloseMouth();
      }

      public async void CloseMouth()
      {
         character.AnimTree.Set("parameters/OpenMouth/add_amount", 0);
         character.AnimTree.Set("parameters/CloseMouth/active", true);
         await Task.Delay(TimeSpan.FromSeconds(.49f));
         character.AnimTree.Set("parameters/CloseMouth/active", false);
         character.openMouth = false;
      }
   }
}