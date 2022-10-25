using Godot;
using System;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.UI
{
    public class BtnMode : Panel
    {
        [Export]
        private GameManager.GameMode gameMode;
        [Export]
        private NodePath thumbPath;
        [Export]
        private NodePath labelWrapperPath;
        private SceneTreeTween _tween;
        public bool IsShowing { get; set; }
        private bool isScaled = false;
        public bool isHovered = false;
        [Inject]
        GameManager gameManager;
        private TextureRect thumb;
        private PanelContainer labelWrapper;
        private Label label;

        public override void _Ready()
        {
            this.ResolveDependencies();
            thumb = GetNode<TextureRect>(thumbPath);
            labelWrapper = GetNode<PanelContainer>(labelWrapperPath);
            label = labelWrapper.GetNode<Label>("Label");
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton)
            {
                if (isHovered) {
                    isHovered = false;
                    OnMouseExited();
                    gameManager.TriggerEvent(GameEvent.GameModeChange, gameMode);
                }
                return;
            }
            if (@event is InputEventMouseMotion && IsShowing)
            {
                bool hasPoint = GetGlobalRect().HasPoint(((InputEventMouseMotion) @event).Position);
                if (hasPoint) {
                    isHovered = true;
                    OnMouseEntered();
                    return;
                }
                if (isHovered) {
                    isHovered = false;
                    OnMouseExited();
                }
            }
        }

        public void OnMouseEntered() 
        {
            if (!IsShowing) return;
            isScaled = true;
            if (_tween != null) _tween.Stop();
            _tween = GetTree().CreateTween();
            _tween.TweenProperty(this, "rect_scale", new Vector2(1.4f, 1.4f), .2f);
            _tween.Parallel().TweenProperty(labelWrapper, "rect_position:y", -labelWrapper.RectSize.y*1.5f, .2f);
            _tween.Parallel().TweenProperty(labelWrapper, "modulate:a", 1.0f, .2f);
            _tween.Play();
        }
        public void OnMouseExited() 
        {
            if (!isScaled) return;
            if (_tween != null) _tween.Stop();
            _tween = GetTree().CreateTween();
            _tween.TweenProperty(this, "rect_scale", new Vector2(1f, 1f), .15f);
            _tween.Parallel().TweenProperty(labelWrapper, "rect_position:y", 0f, .2f);
            _tween.Parallel().TweenProperty(labelWrapper, "modulate:a", 0.0f, .2f);
            _tween.Play();
            isScaled = false;
        }

    }
}