using Godot;
using System;

namespace CrazyEaters.UI
{
    public class GameCreateDotStep : NinePatchRect
    {
        [Export] private string targetId;

        [Signal] public delegate void click(GameCreateDotStep dot);

        private SceneTreeTween tween;

        public string TargetId {
            get {
                return targetId;
            }
        }
        
        public override void _Ready()
        {
            Connect("gui_input", this, nameof(OnDotStepClick));
        }

        public void OnDotStepClick(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                if (((InputEventMouseButton) @event).ButtonIndex != (int) ButtonList.Left) return;
                if (!((InputEventMouseButton) @event).IsPressed()) {
                    EmitSignal("click", this);
                }
            }
        }

        public void ChangeDotColor(Color color)
        {
            if (tween != null) {
                tween.Kill();
            }
            tween = GetTree().CreateTween();
            tween.TweenProperty(this, "self_modulate", color, .4f);
        }

    }
}
