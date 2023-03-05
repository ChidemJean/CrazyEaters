using Godot;
using System;

namespace CrazyEaters.UI 
{
    public class SliderSelectorItem : TextureRect
    {
        [Export] private string key;

        public string Key {
            get {
                return key;
            }
            set {
                key = value;
            }
        }

        public Action onReady;

        [Signal] public delegate void click(SliderSelectorItem item);

        public override void _Ready()
        {
            Connect("gui_input", this, nameof(OnGuiInput));
            OnReady();
        }

        public void OnReady()
        {
            if (onReady != null) onReady.Invoke();
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