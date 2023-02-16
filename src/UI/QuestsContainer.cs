using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;
using CrazyEaters.UI.Generics;

namespace CrazyEaters.UI
{
    public class QuestsContainer : ColorRect
    {
        [Inject] GameManager gameManager;
        [Export] public NodePath panelPath;
        [Export] public NodePath titleContainerPath;
        [Export] public Color bgColorOpen;
        [Export] public NodePath buttonClosePath;

        private ColorRect background;
        private Control panel;
        private Control titleContainer;
        private SceneTreeTween tween;

        private Color initialBgColor;
        private Vector2 initialPosPanel;
        private Control buttonClose;

        public override void _Ready()
        {
            this.ResolveDependencies();
            background = (ColorRect) this;
            panel = GetNode<Control>(panelPath);
            buttonClose = GetNode<BlueButtonNinePatch>(buttonClosePath);
            titleContainer = GetNode<Control>(titleContainerPath);

            gameManager.StartListening(GameEvent.OpenQuestsPanel, Open);
            gameManager.StartListening(GameEvent.CloseQuestsPanel, Close);
            buttonClose.Connect("click", this, nameof(OnClickClose));

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
            tween.Parallel().TweenProperty(panel, "rect_global_position:y", RectSize.y - (panel.RectSize.y), .3f);
            tween.Parallel().TweenProperty(this, "material:shader_param/blur_amount", 3f, .3f);
        }

        public async void Close(object param = null)
        {
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
            tween.TweenProperty(background, "color", initialBgColor, .3f);
            tween.Parallel().TweenProperty(panel, "rect_global_position:y", RectSize.y + titleContainer.RectSize.y, .3f);
            tween.Parallel().TweenProperty(this, "material:shader_param/blur_amount", 0f, .3f);
            await ToSignal(tween, "finished");
            Visible = false;
        }

        public void OnClickClose(bool pressed)
        {
            if (!pressed) Close();
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.OpenQuestsPanel, Open);
            gameManager.StopListening(GameEvent.CloseQuestsPanel, Close);
        }
    }
}