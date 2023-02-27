using Godot;
using System;

namespace CrazyEaters.UI
{
    public class EaterStep : Control
    {
        [Export] private NodePath headerPath;
        [Export] private NodePath footerPath;
        private Control header;
        private Control footer;
        SceneTreeTween tween;

        public override void _Ready()
        {
            header = GetNode<Control>(headerPath);
            footer = GetNode<Control>(footerPath);
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
            GD.Print(GetViewportRect().Size.y);
            tween.Parallel().TweenProperty(footer, "rect_position:y", GetViewportRect().Size.y - footer.RectSize.y, .7f);
        }
    }
}
