using Godot;
using System;
using CrazyEaters.Characters;

namespace CrazyEaters.Enemies
{
   public class Enemy : BaseCharacter
   {
      public override void _Ready()
      {

      }
      public override float IdleAnimationVariations(float idleTime)
      {
         throw new NotImplementedException();
      }

      public override void OnNavmeshChanged()
      {
         throw new NotImplementedException();
      }

      public override void TakeDamage(int damage)
      {

      }

      public override void Die()
      {
		QueueFree();
      }

   }
}