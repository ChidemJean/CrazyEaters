using Godot;
using System;

namespace CrazyEaters.UI
{
    public abstract class GameCreateStep : Control
    {
        [Export] private string id;
        public string keySelected;

        public string Id {
            get {
                return id;
            }
        }

        SceneTreeTween tween;

        public virtual async void Hide(int dir, bool animate)
        {
            if (!animate) {
                Visible = false;
                return;
            }
            if (tween != null) {
                tween.Kill();
            }
            tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
            tween.TweenProperty(this, "modulate:a", 0f, .75f);
            PropertyTweener tweener = tween.Parallel().TweenProperty(this, "rect_position:x", RectSize.x * dir, .95f);
            await ToSignal(tweener, "finished");
            Visible = false;
        }
        public virtual async void Show(int dir, bool animate)
        {
            if (!animate) {
                Visible = true;
                return;
            }
            RectPosition = new Vector2(RectSize.x * dir, RectPosition.y);
            Color mod = Modulate;
            mod.a = 0f;
            Modulate = mod;
            Visible = true;
            //
            if (tween != null) {
                tween.Kill();
            }
            tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
            tween.TweenProperty(this, "modulate:a", 1f, .75f);
            tween.Parallel().TweenProperty(this, "rect_position:x", 0f, .95f);
        }
    }
}