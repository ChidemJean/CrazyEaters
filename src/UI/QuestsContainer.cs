using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class QuestsContainer : ColorRect
    {
        [Inject] GameManager gameManager;
        [Export] public NodePath panelPath;
        [Export] public NodePath titleContainerPath;
        [Export] public Color bgColorOpen;

        private ColorRect background;
        private Control panel;
        private Control titleContainer;
        private SceneTreeTween tween;

        private Color initialBgColor;
        private Vector2 initialPosPanel;

        public override void _Ready()
        {
            this.ResolveDependencies();
            background = (ColorRect) this;
            panel = GetNode<Control>(panelPath);
            titleContainer = GetNode<Control>(titleContainerPath);
            gameManager.StartListening(GameEvent.OpenQuestsPanel, Open);

            initialBgColor = background.Color;
            initialPosPanel = panel.RectGlobalPosition;
            initialPosPanel = panel.RectGlobalPosition;
        }

        public async void Open(object param)
        {
            Visible = true;
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
            tween.TweenProperty(background, "color", bgColorOpen, .3f);
            tween.TweenProperty(panel, "rect_global_position:y", RectSize.y - (titleContainer.RectSize.y + panel.RectSize.y), .3f);
        }

        public async void Close()
        {
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
            tween.TweenProperty(background, "color", initialBgColor, .3f);
            tween.TweenProperty(panel, "rect_global_position:y", RectSize.y + titleContainer.RectSize.y, .3f);
            await ToSignal(tween, "finished");
            Visible = false;
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.OpenQuestsPanel, Open);
        }
    }
}