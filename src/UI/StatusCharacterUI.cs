namespace CrazyEaters.UI
{
    using Godot;
    using System;
    using CrazyEaters.Resources;
    using CrazyEaters.DependencyInjection;
    using CrazyEaters.Managers;

    public class StatusCharacterUI : Control
    {
        private StatusCharacter statusData;
        public StatusCharacter StatusData { 
            get {
                return statusData;
            } 
            set {
                statusData = value;
                if (IsInsideTree()) OnStatusUpdate();
            }
        }

        [Export]
        private NodePath iconPath;

        [Export]
        private NodePath numeroPath;

        [Export]
        private NodePath clipPanelPath;

        [Inject]
        public GameManager gameManager;

        private Label numeroLabel;
        private TextureRect icon;
        private Control clipPanel;
        private int value = 0;
        
        private Vector2 clipMaxSize;
        private Tween tween;

        public override void _Ready()
        {
            this.ResolveDependencies();
            tween = new Tween();
            AddChild(tween);
            icon = GetNode<TextureRect>(iconPath);
            numeroLabel = GetNode<Label>(numeroPath);
            clipPanel = GetNode<Control>(clipPanelPath);
            clipMaxSize = clipPanel.RectSize;

            gameManager.StartListening(GameEvent.UpdateCharacterStatus, OnUpdateStatusCharacter);
            OnStatusUpdate();
        }

        private void OnStatusUpdate()
        {
            value = value == 0 ? statusData.max : 0;
            UpdateUI();
        }

        private void OnUpdateStatusCharacter(object data)
        {
            if (!(data is CharacterStatusEventData)) {
                return;
            }
            CharacterStatusEventData _data = (CharacterStatusEventData) data;
            if (_data.name == statusData.key) {
                UpdateValue(_data.newValue);
            }
        }

        private void UpdateUI()
        {
            icon.Texture = statusData.icon;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            numeroLabel.Text = GetPercent() + "%";
        }

        private float GetPercent()
        {
            return value * 100 / statusData.max;
        }

        public void UpdateValue(int newValue)
        {
            value = Mathf.Clamp(value + newValue, 0, statusData.max);
            UpdateLabel();
            AnimateClip();
        }

        public void AnimateClip()
        {
            tween.StopAll();
            float toValue = clipMaxSize.y * ((float) value / (float)statusData.max);
            tween.InterpolateProperty(clipPanel, "rect_size:y", clipPanel.RectSize.y, toValue, .8f);
            tween.InterpolateProperty(clipPanel, "rect_position:y", clipPanel.RectPosition.y, clipMaxSize.y - toValue, .8f);
            tween.Start();
        }

    }
}