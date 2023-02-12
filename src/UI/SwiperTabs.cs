using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI 
{
    public class SwiperTabs : Control
    {
        [Export]
        public NodePath tabContainerPath;

        [Export]
        public NodePath targetRailPath;
        [Inject] 
        private GameManager gameManager;
        public Control tabContainer;
        public Control targetRail;
        public Godot.Collections.Array tabs;
        public Godot.Collections.Array panels;
        // public Godot.Collections.Dictionary<string, PanelTabItem> panels;
        private bool pressed = false;
        private bool isMoving = false;
        private float percentOffset = .15f;

        SceneTreeTween tween;

        public override void _Ready()
        {
            this.ResolveDependencies();
            targetRail = GetNode<Control>(targetRailPath);
            tabContainer = GetNode<Control>(tabContainerPath);
            PopulateTabs();
            Connect("gui_input", this, nameof(OnGuiInput));
            gameManager.StartListening(GameEvent.MenuBottomItemClick, OnItemClick);
        }

        public void PopulateTabs()
        {
            tabs = tabContainer.GetChildren();
            //
            panels = targetRail.GetChildren();
            // panels = new Godot.Collections.Dictionary<string, PanelTabItem>();
            // foreach (Control _panel in targetRail.GetChildren()) {
            //     PanelTabItem panel = (PanelTabItem) _panel;
            //     panels.Add(panel.key, panel);
            // }
        }

        public void OnItemClick(object param)
        {
            AnimateToPanel((string) param);
        }

        public void OnGuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                pressed = ((InputEventMouseButton) @event).IsPressed();
                if (!pressed) AnimateToPanel();
                return;
            }
            if (pressed && @event is InputEventMouseMotion) {
                InputEventMouseMotion motionEvent = (InputEventMouseMotion) @event;
                Move(motionEvent.Relative);
            }
        }

        public void Move(Vector2 motion) 
        {
            isMoving = true;
            
            float max = (targetRail.RectSize.x - RectSize.x);
            float newX = targetRail.RectGlobalPosition.x + motion.x;
            float negOffset = RectSize.x * percentOffset;
            float tempNewXNeg = (newX < -max) ? (motion.x * ((Mathf.Abs(newX) - max)/negOffset)) : 0;
            float tempNewXPos = (newX > 0) ? (motion.x * (newX/negOffset)) : 0;

            newX -= (tempNewXNeg == 0 ? tempNewXPos : tempNewXNeg);

            float newXClamped = Mathf.Clamp(newX, - max - negOffset, negOffset);
            targetRail.RectGlobalPosition = new Vector2(newXClamped, targetRail.RectGlobalPosition.y);
        }

        public async void AnimateToPanel(string key = null)
        {
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

            int toIndex = key != null ? GetIndexPanelByKey(key) : Mathf.Clamp(Mathf.RoundToInt(targetRail.RectGlobalPosition.x * -1 / RectSize.x), 0, panels.Count - 1);

            if (key == null) {
                key = ((PanelTabItem)panels[toIndex]).key;
            }
            gameManager.TriggerEvent(GameEvent.ChangePanel, key);

            var pT = tween.TweenProperty(targetRail, "rect_global_position:x", toIndex * RectSize.x * -1, .4f);
            await ToSignal(pT, "finished");
            isMoving = false;
        }

        public int GetIndexPanelByKey(string key)
        {
            for(int i = 0; i < panels.Count; i++) {
                PanelTabItem tab = (PanelTabItem) panels[i];
                if (tab.key == key) {
                    return i;
                }
            }
            return 0;
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.MenuBottomItemClick, OnItemClick);
        }
    }
}