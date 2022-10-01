using Godot;
using System;

namespace CrazyEaters.AI.StateMachine
{
    public interface IState
    {
        void Tick(float delta);
        void FixedTick();
        void OnEnter();
        void OnExit();
    }
}