using Godot;
using System;

namespace CrazyEaters.AI.StateMachine
{
    public interface IState
    {
        void Tick();
        void FixedTick();
        void OnEnter();
        void OnExit();
    }
}