using Godot;
using System;
using CrazyEaters.Managers;
using CrazyEaters.Characters;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Characters.States;
using CrazyEaters.Enemies.States;

namespace CrazyEaters.Enemies
{
   public class Enemy : BaseCharacter
   {
      [Inject] private SceneSwitcher sceneSwitcher;
      public bool isFollowing = false;

      public override void _Ready()
      {
         this.ResolveDependencies();
         base._Ready();

         ai_StateMachine.AddAnyTransition(new FollowState(this), () =>
         {
            return !GetTargetCharacter().isDead && !isFollowing;
         });
         ai_StateMachine.AddAnyTransition(new OpenMouthState(this), () =>
         {
            return openMouth;
         });
      }

      public Character GetTargetCharacter()
      {
         return (sceneSwitcher.currentScene as HabitatScene).Character;
      }
      
      public override float IdleAnimationVariations(float idleTime)
      {
         return 6;
      }

      public override void OnNavmeshChanged()
      {
      }

      public override void TakeDamage(int damage)
      {

      }

      public override void _PhysicsProcess(float delta)
      {
         base._PhysicsProcess(delta);
      }

      public override void Die()
      {
		   QueueFree();
      }

   }
}