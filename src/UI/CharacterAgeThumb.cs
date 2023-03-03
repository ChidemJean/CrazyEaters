using Godot;
using System;

namespace CrazyEaters.UI
{
    public class CharacterAgeThumb : TextureRect
    {
        [Export] public Color modulateOnSelected;
        [Signal] public delegate void click(string key);

        public string key;
        private Color initialModulate;

        public override void _Ready()
        {
            Connect("gui_input", this, nameof(OnItemGuiInput));
            initialModulate = Modulate;
        }
        public void OnItemGuiInput(InputEvent @event) 
        {
            if (!(@event is InputEventMouseButton)) return;
            InputEventMouseButton _event = (InputEventMouseButton) @event;
            if (_event.IsPressed()) return;
            EmitSignal("click", key);
        }
        public void Select()
        {
            Modulate = modulateOnSelected;
        }
        public void ResetModulate()
        {
            Modulate = initialModulate;
        }
    }
}
