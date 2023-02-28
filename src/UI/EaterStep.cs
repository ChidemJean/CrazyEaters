using Godot;
using System;

namespace CrazyEaters.UI
{
    public class EaterStep : GameCreateStep
    {
        [Export] private NodePath headerPath;
        private Control header;
        SceneTreeTween tween;

        public override void _Ready()
        {
            header = GetNode<Control>(headerPath);
            header.RectPosition = new Vector2(0, -header.GetNode<Control>("Main").RectSize.y);
            Show();
        }

        public void Show()
        {
            if (tween != null) {
                tween.Kill();
            }
            tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            tween.TweenProperty(header, "rect_position:y", 0f, .95f);
        }

        public override async void Hide(int dir, bool animate)
        {
            base.Hide(dir, animate);
        }
        public override async void Show(int dir, bool animate)
        {
            base.Show(dir, animate);
        }
    }
}
