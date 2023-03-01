using Godot;
using System;

namespace CrazyEaters.UI 
{
    public class SliderSelectorItem : Control
    {
        [Export] private string key;

        public string Key {
            get {
                return key;
            }
        }

        [Signal] public delegate void click(SliderSelectorItem item);

        public override void _Ready()
        {
            Connect("gui_input", this, nameof(OnGuiInput));
        }

        public void OnGuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                if (((InputEventMouseButton) @event).ButtonIndex != (int) ButtonList.Left) return;
                if (!((InputEventMouseButton) @event).IsPressed()) {
                    EmitSignal("click", this);
                }
            }
        }

    }
}