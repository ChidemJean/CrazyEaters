using Godot;
using System;
using CrazyEaters.AI.StateMachine;

namespace CrazyEaters.Characters.States
{

    public class BaseState : IState
    {
        protected BaseCharacter character;
        protected bool active = false;

        public BaseState(BaseCharacter baseCharacter)
        {
            this.character = baseCharacter;
        }

        public virtual void FixedTick()
        {


        }

        public virtual void OnEnter()
        {
            
        }

        public virtual void OnExit()
        {

        }

        public virtual void Tick(float delta)
        {

        }
    }
}