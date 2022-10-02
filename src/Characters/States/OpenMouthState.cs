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
         if (!active) {
            SceneTreeTween tween = character.GetTree().CreateTween();
            tween.TweenProperty(character.animationTree, "parameters/OpenMouth/add_amount", 1f, .15f)?.SetEase(Tween.EaseType.Out);
            Timeout();
            character.GM.StartListening(Managers.GameEvent.FoodEatFinish, OnEatEnd);
            active = true;
         }
      }

      public override void OnExit()
      {
         base.OnExit();
         character.GM.StopListening(Managers.GameEvent.FoodEatFinish, OnEatEnd);
      }

      private async void Timeout()
      {
         await Task.Delay(TimeSpan.FromSeconds(4));
         if (character.openMouth) CloseMouth();
      }

      public void OnEatEnd(object param)
      {
         if (character.GetInstanceId() == ((BaseCharacter) param).GetInstanceId()) {
            character.AnimTree.Set("parameters/OpenMouth/add_amount", 0);
            character.openMouth = false;
            active = false;
         }
      }

      public async void CloseMouth()
      {
         SceneTreeTween tween = character.GetTree().CreateTween();
         tween.TweenProperty(character.AnimTree, "parameters/OpenMouth/add_amount", 0f, .5f)?.SetEase(Tween.EaseType.Out);
         await character.ToSignal(tween, "finished");
         character.openMouth = false;
         active = false;
      }
   }
}