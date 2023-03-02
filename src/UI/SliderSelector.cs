using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Godot.Collections;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI 
{
    public class SliderSelector : Control
    {
        [Export] public string key;
        [Export]
        public NodePath targetRailPath;
        [Inject] 
        private GameManager gameManager;
        public Control targetRail;
        public Godot.Collections.Array panels;
        private bool pressed = false;
        private bool isMoving = false;
        private float percentOffset = .15f;

        [Export] private float minWidth = 300f;
        [Export] private float maxWidth = 450f;
        [Export] private float proximityToScale = .30f;
        [Export] private float scaleSelected = 1.35f;
        [Export] private NodePath labelCurrentPath;
        [Export] private NodePath labelTotalPath;

        private Label labelCurrent, labelTotal;

        SceneTreeTween tween;
        private int currentIndex = 0;
        public int CurrentIndex {
            get {
                return currentIndex;
            }
            set {
                currentIndex = value;
                SelectedChanged();
            }
        }

        [Signal] public delegate void slideChange(string key);

        public override void _Ready()
        {
            this.ResolveDependencies();
            labelCurrent = GetNode<Label>(labelCurrentPath);
            labelTotal = GetNode<Label>(labelTotalPath);
            targetRail = GetNode<Control>(targetRailPath);
            PopulateTabs();
            Connect("gui_input", this, nameof(OnGuiInput));
        }

        public void PopulateTabs()
        {
            panels = targetRail.GetChildren();
            foreach (SliderSelectorItem panel in panels) {
                panel.Connect("click", this, nameof(OnItemClick));
            }
            labelTotal.Text = panels.Count.ToString();
            AnimateToPanel();
        }

        public void OnItemClick(SliderSelectorItem item)
        {
            AnimateToPanel(item.Key);
        }

        public void OnGuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                if (((InputEventMouseButton) @event).ButtonIndex != (int) ButtonList.Left) return;
                pressed = ((InputEventMouseButton) @event).IsPressed();
                if (!pressed && isMoving) AnimateToPanel();
                return;
            }
            if (pressed && @event is InputEventMouseMotion) {
                InputEventMouseMotion motionEvent = (InputEventMouseMotion) @event;
                Move(motionEvent.Relative);
            }
        }

        public void UpdateItemSizes()
        {
            float midScreenX = RectSize.x / 2f;
            float rangeXToScale = minWidth * proximityToScale;
            foreach (SliderSelectorItem item in panels) {
                float centerItemPoint = item.RectGlobalPosition.x + item.RectSize.x/2;
                float difToMid = Mathf.Abs(centerItemPoint - midScreenX);
                // if (difToMid <= rangeXToScale) {
                    float percent = (1f - (difToMid*proximityToScale)/midScreenX);
                    float newSizeX = Mathf.Clamp(maxWidth * percent, minWidth, maxWidth);
                    // item.RectMinSize = new Vector2(newSizeX, item.RectSize.y);
                    float scaleUnit = Mathf.Clamp(scaleSelected * percent, 1f, scaleSelected);
                    item.RectScale = new Vector2(scaleUnit, scaleUnit);
                    float colorUnit = Mathf.Clamp(1f * percent, .25f, 1f);
                    item.Modulate = new Color(colorUnit, colorUnit, colorUnit, colorUnit);
                // } else {
                //     item.RectMinSize = new Vector2(minWidth, item.RectSize.y);
                // }
            }
        }

        public void Move(Vector2 motion) 
        {
            UpdateItemSizes();

            isMoving = true;
            
            float max = (targetRail.RectSize.x - RectSize.x) * 1.15f;
            float newX = targetRail.RectGlobalPosition.x + motion.x;
            float negOffset = RectSize.x * percentOffset * 4f;
            float tempNewXNeg = (newX < -max) ? (motion.x * ((Mathf.Abs(newX) - max)/negOffset)) : 0;
            float tempNewXPos = (newX > 0) ? (motion.x * (newX/negOffset)) : 0;

            newX -= (tempNewXNeg == 0 ? tempNewXPos : tempNewXNeg);

            float newXClamped = Mathf.Clamp(newX, - max - negOffset, negOffset);
            targetRail.RectGlobalPosition = new Vector2(newXClamped, targetRail.RectGlobalPosition.y);
        }

        public float GetMediaItemSizes()
        {
            float media = 0;
            foreach (SliderSelectorItem item in panels) {
                media += item.RectSize.x;
            }
            return media/panels.Count;
        }

        public void MoveAnimation(float value)
        {
            UpdateItemSizes();
            Vector2 vec = targetRail.RectGlobalPosition;
            vec.x = value;
            targetRail.RectGlobalPosition = vec;
        }

        public int GetIdxNearestItemToMiddle()
        {
            float midScreenX = RectSize.x / 2f;
            float minDif = -1;
            int idx = 0;
            for (int i = 0; i < panels.Count; i++) {
                SliderSelectorItem item = (SliderSelectorItem) panels[i];
                float centerItemPoint = item.RectGlobalPosition.x + item.RectSize.x/2;
                float difToMid = Mathf.Abs(centerItemPoint - midScreenX);
                if (minDif == -1 || difToMid < minDif) {
                    minDif = difToMid;
                    idx = i;
                }
            }
            return idx;
        }

        public async void AnimateToPanel(string key = null)
        {
            if (panels.Count == 0) return;
            float midScreenX = RectSize.x / 2f;
            float totalGaps = targetRail.RectSize.x - minWidth * panels.Count;
            float gap = (totalGaps / (panels.Count + 1));
            float mediaWidthItem = minWidth + gap;

            int toIndex = key != null ? GetIndexPanelByKey(key) : GetIdxNearestItemToMiddle();
            if (key == null) {
                key = ((SliderSelectorItem)panels[toIndex]).Key;
            }

            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

            SliderSelectorItem item = (SliderSelectorItem) panels[toIndex];
            float moveX = midScreenX - (item.RectGlobalPosition.x + item.RectSize.x/2 + gap/2);
            var pT = tween.TweenMethod(this, nameof(MoveAnimation), targetRail.RectGlobalPosition.x, targetRail.RectGlobalPosition.x + moveX, .4f);
            await ToSignal(pT, "finished");
            isMoving = false;
            CurrentIndex = toIndex;
        }

        public int GetIndexPanelByKey(string key)
        {
            for(int i = 0; i < panels.Count; i++) {
                SliderSelectorItem tab = (SliderSelectorItem) panels[i];
                if (tab.Key == key) {
                    return i;
                }
            }
            return 0;
        }

        public void SelectedChanged()
        {
            labelCurrent.Text = (currentIndex + 1).ToString();
            string keyPanel = ((SliderSelectorItem) panels[currentIndex]).Key;
            EmitSignal("slideChange", keyPanel);
            // gameManager.TriggerEvent(GameEvent.SliderSelectorChange, $"{key}-{keyPanel}");
        }
    }
}