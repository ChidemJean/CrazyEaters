using Godot;
using System;

namespace CrazyEaters.UI
{
    public class CategoryTabButton : TextureButton
    {
        [Signal] public delegate void click(string name);
        
        public override void _Ready()
        {
            Connect("gui_input", this, nameof(OnItemGuiInput));
        }

        public void OnItemGuiInput(InputEvent @event) 
        {
            if (!(@event is InputEventMouseButton)) return;
            InputEventMouseButton _event = (InputEventMouseButton) @event;
            if (_event.IsPressed()) return;
            EmitSignal("click", Name);
        }
    }
}
