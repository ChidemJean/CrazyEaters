using Godot;
using System;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.UI
{
    public class NewGameButton : TextureButton
    {
        [Inject] GameManager gameManager;

        public override void _Ready()
        {
            this.ResolveDependencies();
            Connect("button_up", this, nameof(OnClick));
        }
        public void OnClick()
        {
            gameManager.TriggerEvent(GameEvent.ChangeScene, "game_create");
        }
    }
}