using Godot;
using System;
using CrazyEaters.AI.StateMachine;
using CrazyEaters.Characters;
using CrazyEaters.Characters.States;

namespace CrazyEaters.Enemies.States
{

   public class FollowState : BaseState
   {
      protected float passedTime = 0.0f;

      public FollowState(Enemy enemy) : base(enemy)
      {
      }

      public override void OnEnter()
      {
         base.OnEnter();
         ((Enemy)character).isFollowing = true;
         character.AnimTree.Set("parameters/Walk/blend_amount", 1);
         character.AnimTree.Set("parameters/State/current", 0);
      }

      public override void OnExit()
      {
         base.OnExit();
         ((Enemy)character).isFollowing = false;
         character.AnimTree.Set("parameters/Walk/blend_amount", 0);
      }

      public override void Tick(float delta)
      {
         base.Tick(delta);
         passedTime += delta;
         if (passedTime > 2)
         {
            var targetPos = ((Enemy)character).GetTargetCharacter().GlobalTransform.origin;
            if (((Enemy)character).targetIALocation != targetPos)
            {
               ((Enemy)character).targetIALocation = targetPos;
               ((Enemy)character).UpdatePath();
               passedTime = 0;
            }
         }
      }
   }
}