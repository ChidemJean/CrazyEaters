namespace CrazyEaters.UI 
{
    using Godot;
    using System;
    using CrazyEaters.Optimization;
    using CrazyEaters.Managers;

    public class ConfigurationsMenu : PanelContainer
    {
        [Export]
        public NodePath closeButtonPath = "MarginContainer/Control/CloseButton";

        [Export]
        public NodePath renderSliderPath = "MarginContainer2/ScrollContainer/Control/ConfLine/RenderScaleSlider";
        
        [Export]
        public NodePath valueLabelPath = "MarginContainer2/ScrollContainer/Control/ConfLine/ValueLabel";

        [Export]
        public NodePath hudPath = "/root/MainNode";

        bool open = false;
        TextureButton closeButton;
        HSlider renderSlider;

        Hud hud;
        Label valueLabel;
        GameManager gameManager;

        public override void _Ready()
        {
            Visible = false;
            gameManager = GetNode<GameManager>("/root/GameManager");

            valueLabel = GetNode<Label>(valueLabelPath);

            hud = GetNode<Hud>(hudPath);
            hud?.Connect("OnUpdateViewport3DSize", this, nameof(OnUpdateViewport3DScale));

            closeButton = GetNode<TextureButton>(closeButtonPath);
            closeButton?.Connect("button_up", this, nameof(Hide));

            renderSlider = GetNode<HSlider>(renderSliderPath);

            if (renderSlider != null) {
                renderSlider?.Connect("value_changed", this, nameof(OnSliderScaleChanged));
            }
        }

        public void Show() {
            this.Visible = true;
            gameManager.inputMode = GameManager.InputMode.UI;
        }

        public void Hide() {
            this.Visible = false;
            gameManager.inputMode = GameManager.InputMode.SCENE;
        }

        public void OnUpdateViewport3DScale(string type, float value) {
            float finalScale = value * 100;
            if (type != "OPTION") {
                renderSlider.Value = finalScale;
            }
            valueLabel.Text = (int)finalScale + "/" + 100;
        }

        public void OnSliderScaleChanged(float value) {
            float renderScaleVal = value / 100;
            hud.UpdateViewport3DSize(renderScaleVal, "OPTION");
        }

    }
}
