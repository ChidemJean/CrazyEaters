using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.Animations
{
    public class DayNightAnimation : AnimationPlayer
    {
        [Inject] GameManager gameManager;

        public override void _Ready()
        {
            this.ResolveDependencies();
        }

        public void OnNight() 
        {
            gameManager.TriggerEvent(GameEvent.InNight, "");
        }

        public void OnDay() 
        {
            gameManager.TriggerEvent(GameEvent.InDay, "");
        }
    }
}