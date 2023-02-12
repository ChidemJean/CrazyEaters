using Godot;
using System;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.UI
{

    public class HighlightMenuSelected : Control
    {
        [Inject]
        GameManager gameManager;

        private Vector2 initialPosition;
        private Vector2 initialSize;
        SceneTreeTween tween;
        public override void _Ready()
        {
            this.ResolveDependencies();
            gameManager.StartListening(GameEvent.ChangePanel, OnPanelChange);
            initialPosition = RectPosition;
            initialSize = RectSize;
        }

        public void OnPanelChange(object param)
        {
            ChangePanelEventData data = (ChangePanelEventData) param;
            RectPosition = new Vector2(initialPosition.x + (initialSize.x) * data.index + initialPosition.x * data.index * 2.14f, -initialPosition.y * 2f);
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
            tween.TweenProperty(this, "rect_position:y", initialPosition.y, .26f);
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.ChangePanel, OnPanelChange);
        }
    }
}