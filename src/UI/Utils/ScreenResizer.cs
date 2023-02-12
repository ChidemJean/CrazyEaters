using Godot;
using System;

namespace CrazyEaters.UI.Utils 
{
    public class ScreenResizer : Node
    {
        [Export]
        public NodePath fullRectGuiderPath;
        public Control fullRectGuider;
        private Control target;
        public override void _Ready()
        {
            fullRectGuider = GetNode<Control>(fullRectGuiderPath);
            target = GetParentOrNull<Control>();
            fullRectGuider.Connect("resized", this, nameof(ApplyWindowWidth));
            GetTree().Root.Connect("size_changed", this, nameof(ApplyWindowWidth));
        }

        public void ApplyWindowWidth()
        {
            if (target != null) {
                target.RectMinSize = new Vector2(fullRectGuider.RectSize.x, fullRectGuider.RectSize.y);
                target.RectSize = new Vector2(fullRectGuider.RectSize.x, fullRectGuider.RectSize.y);
            }
        }
    }
}