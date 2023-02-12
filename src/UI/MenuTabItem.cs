using Godot;
using System;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.UI
{
    public class MenuTabItem : Control
    {
        [Export]
        public string tabPanelKey;
        [Export]
        public NodePath bgPath;
        [Export]
        public NodePath labelPath;
        [Export]
        public NodePath shinePath;
        [Export]
        public NodePath iconPath;

        private Label label;
        private TextureRect shine;
        private TextureRect icon;
        private Control bg;

        [Inject] 
        private GameManager gameManager;

        [Export]
        public bool active = false;

        SceneTreeTween tween;
        float initialMarginTop;
        float initialIconY;
        float initialLabelY;
        Color initialSelfModulateBg;
        Color initialSelfModulateShine;
        Vector2 initialScale;

        public override void _Ready()
        {
            this.ResolveDependencies();
            label = GetNode<Label>(labelPath);
            shine = GetNode<TextureRect>(shinePath);
            icon = GetNode<TextureRect>(iconPath);
            bg = GetNode<Control>(bgPath);
            Connect("gui_input", this, nameof(OnItemGuiInput));
            gameManager.StartListening(GameEvent.ChangePanel, OnPanelChange);
            initialMarginTop = bg.MarginTop;
            initialSelfModulateBg = bg.SelfModulate;
            initialSelfModulateShine = shine.SelfModulate;
            initialIconY = icon.RectPosition.y;
            initialLabelY = label.RectPosition.y;
            initialScale = icon.RectScale;
        }

        public void OnPanelChange(object param)
        {
            ChangePanelEventData data = (ChangePanelEventData) param;
            if (data.key == tabPanelKey) {
                Activate();
                return;
            }
            if (active) {
                Deactivate();
            }
        }

        public async void Activate()
        {
            if (active) return;
            active = true;
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
            tween.TweenProperty(bg, "margin_top", initialMarginTop * 1.3f, .3f);
            tween.Parallel().TweenProperty(bg, "self_modulate", new Color(1,1,1,1), .3f);
            tween.Parallel().TweenProperty(shine, "self_modulate:a", .4f, .3f);
            tween.Parallel().TweenProperty(icon, "rect_position:y", initialIconY * 1.2f, .3f);
            tween.Parallel().TweenProperty(icon, "rect_scale", new Vector2(1.15f, 1.15f), .4f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            tween.Parallel().TweenProperty(label, "self_modulate:a", 1f, .3f);
            tween.Parallel().TweenProperty(label, "rect_position:y", initialLabelY * .82f, .3f);
        }
        public async void Deactivate()
        {
            active = false;
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
            tween.TweenProperty(bg, "margin_top", initialMarginTop, .25f);
            tween.Parallel().TweenProperty(bg, "self_modulate", initialSelfModulateBg, .25f);
            tween.Parallel().TweenProperty(shine, "self_modulate:a", initialSelfModulateShine.a, .25f);
            tween.Parallel().TweenProperty(icon, "rect_position:y", initialIconY, .25f);
            tween.Parallel().TweenProperty(icon, "rect_scale", initialScale, .25f);
            tween.Parallel().TweenProperty(label, "self_modulate:a", 0f, .25f);
            tween.Parallel().TweenProperty(label, "rect_position:y", initialLabelY, .25f);
        }

        public void OnItemGuiInput(InputEvent @event) 
        {
            if (!(@event is InputEventMouseButton)) return;
            InputEventMouseButton _event = (InputEventMouseButton) @event;
            gameManager.TriggerEvent(GameEvent.MenuBottomItemClick, tabPanelKey);
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.ChangePanel, OnPanelChange);
        }
    }
}