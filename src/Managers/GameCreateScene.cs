using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using CrazyEaters.UI;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;
using CrazyEaters.Save;

namespace CrazyEaters.Managers
{
    public class GameCreateScene : CEScene
    {
        [Export] private NodePath closeBtnPath;
        [Export] private NodePath footerPath;
        [Export] private NodePath backBtnPath;
        [Export] private NodePath nextBtnPath;
        [Export] private NodePath finishBtnPath;
        [Export] private Color dotNormalColor;
        [Export] private Color dotSelectedColor;
        [Export] private Color dotCompletedColor;
        [Inject] SaveSystemNode saveSystemNode;

        private TextureButton closeBtn;
        SceneTreeTween tween;
        private Control footer;
        private Control backBtn;
        private Control nextBtn;
        private Control finishBtn;
        private List<GameCreateStep> steps;
        private List<GameCreateDotStep> dotsSteps;
        private int currentStep = 0;
        private HabitatsGameData habitatsGameData = null;

        public override void _Ready()
        {
            this.ResolveDependencies();
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
                dotStep.Connect("click", this, nameof(OnDotStepClick));
            }

            Show();
            ChangeStep(currentStep, false);
            saveSystemNode.LoadHabitats(OnHabitatsLoaded);
        }

        public void OnHabitatsLoaded(HabitatsGameData habitats)
        {
            habitatsGameData = habitats;
        }

        public void OnDotStepClick(GameCreateDotStep dotStep)
        {
            if (dotStep == null) return;
            ChangeStepByKey(dotStep.TargetId);
        }

        public void ChangeStepByKey(string key)
        {
            foreach (var (step, i) in steps.Select((step, i) => (step, i))) {
                if (step.Id == key) {
                    ChangeStep(i);
                }
            }
        }

        public void ChangeStep(int idx, bool animate = true)
        {
            if (currentStep != idx) {
                GameCreateStep curStep = steps[currentStep];
                curStep.Hide(currentStep < idx ? -1 : 1, animate);
            }
            GameCreateStep newStep = steps[idx];
            newStep.Show(currentStep < idx ? 1 : -1, animate);
            currentStep = idx;
            UpdateDotsColors();
            UpdateButtons();
        }

        public void UpdateDotsColors()
        {
            foreach (var (dot, i) in dotsSteps.Select((dot, i) => (dot, i))) {
                if (i == currentStep) {
                    dot.ChangeDotColor(dotSelectedColor);
                    continue;
                }
                if (i < currentStep) {
                    dot.ChangeDotColor(dotCompletedColor);
                    continue;
                }
                if (i > currentStep) {
                    dot.ChangeDotColor(dotNormalColor);
                }
            }
        }

        public void UpdateButtons()
        {
            backBtn.Visible = currentStep == 0 ? false : true;
            nextBtn.Visible = currentStep < (steps.Count - 1) ? true : false;
            finishBtn.Visible = currentStep == (steps.Count - 1) ? true : false;
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
            if (habitatsGameData != null && habitatsGameData.habitatGameData != null && habitatsGameData.habitatGameData.Count > 0) {
                gameManager.TriggerEvent(GameEvent.ChangeScene, "home");
            }
        }

        public void OnBackClick(bool pressed)
        {
            if (pressed) return;
            PrevStep();
        }
        public void OnNextClick(bool pressed)
        {
            if (pressed) return;
            NextStep();
        }
        public void OnFinishClick(bool pressed)
        {
            if (pressed) return;
            InitNewGame();
        }

        public void PrevStep()
        {
            ChangeStep(Mathf.Clamp(currentStep - 1, 0, steps.Count - 1));
        }

        public void NextStep()
        {
            ChangeStep(Mathf.Clamp(currentStep + 1, 0, steps.Count - 1));
        }

        public void InitNewGame()
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