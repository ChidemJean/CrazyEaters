using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class OpenQuestsButton : TextureRect
    {
        [Inject] GameManager gameManager;

        public override void _Ready()
        {
            this.ResolveDependencies();
            Connect("gui_input", this, nameof(OnGuiInput));
        }

        public void OnGuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                InputEventMouseButton _event = (InputEventMouseButton) @event;
                if (_event.ButtonIndex == 0 && !_event.IsPressed()) {
                    gameManager.TriggerEvent(GameEvent.OpenQuestsPanel, "");
                }
            }
        }
    }
}