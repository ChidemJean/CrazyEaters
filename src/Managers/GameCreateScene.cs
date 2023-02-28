using Godot;
using System;
using System.Collections.Generic;
using CrazyEaters.UI;

namespace CrazyEaters.Managers
{
    public class GameCreateScene : CEScene
    {
        [Export] private NodePath closeBtnPath;
        [Export] private NodePath footerPath;
        [Export] private NodePath backBtnPath;
        [Export] private NodePath nextBtnPath;
        [Export] private NodePath finishBtnPath;

        private TextureButton closeBtn;
        SceneTreeTween tween;
        private Control footer;
        private Control backBtn;
        private Control nextBtn;
        private Control finishBtn;
        private List<GameCreateStep> steps;
        private List<GameCreateDotStep> dotsSteps;

        public override void _Ready()
        {
            nextBtn = GetNode<Control>(nextBtnPath);
            backBtn = GetNode<Control>(backBtnPath);
            finishBtn = GetNode<Control>(finishBtnPath);
            closeBtn = GetNode<TextureButton>(closeBtnPath);
            footer = GetNode<Control>(footerPath);
            footer.RectPosition = new Vector2(0, GetViewportRect().Size.y);

            closeBtn.Connect("button_up", this, nameof(OnCloseClick));
            backBtn.Connect("click", this, nameof(OnBackClick));
            nextBtn.Connect("click", this, nameof(OnNextClick));
            finishBtn.Connect("click", this, nameof(OnFinishClick));
            
            steps = new List<GameCreateStep>();
            dotsSteps = new List<GameCreateDotStep>();

            PopulateSteps(this);
            PopulateStepsDots(this);
            foreach (GameCreateDotStep dotStep in dotsSteps) {
            GD.Print(dotsSteps.Count);
                dotStep.Connect("click", this, nameof(OnDotStepClick));
            }

            Show();
        }

        public void OnDotStepClick(GameCreateDotStep dotStep)
        {
            if (dotStep == null) return;
            ChangeStep(dotStep.TargetId);
        }

        public void ChangeStep(string key)
        {
            GD.Print($"CLICK CHANGED: {key}");
        }

        public void PopulateSteps(Node node)
        {
            if (node is GameCreateStep) {
                steps.Add((GameCreateStep)node);
            }
            foreach (Node child in node.GetChildren()) {
                PopulateSteps(child);
            }
        }
        public void PopulateStepsDots(Node node)
        {
            if (node is GameCreateDotStep) {
                dotsSteps.Add((GameCreateDotStep)node);
            }
            foreach (Node child in node.GetChildren()) {
                PopulateStepsDots(child);
            }
        }

        public void OnCloseClick()
        {
            gameManager.TriggerEvent(GameEvent.ChangeScene, "home");
        }

        public void OnBackClick(bool pressed)
        {
            if (pressed) return;
            GD.Print("back click");
        }
        public void OnNextClick(bool pressed)
        {

        }
        public void OnFinishClick(bool pressed)
        {

        }

        public void Show()
        {
            if (tween != null) {
                tween.Kill();
            }
            tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            tween.TweenProperty(footer, "rect_position:y", GetViewportRect().Size.y - footer.RectSize.y, .7f);
        }
    }
}