namespace CrazyEaters.UI 
{
    using Godot;
    using System;
    using CrazyEaters.Optimization;
    using CrazyEaters.Managers;
    using CrazyEaters.Save;

    public class ConfigurationsMenu : PanelContainer
    {
        [Export]
        public NodePath closeButtonPath = "MarginContainer/Control/CloseButton";

        [Export]
        public NodePath renderSliderPath = "MarginContainer2/ScrollContainer/Control/ConfLine/RenderScaleSlider";
        
        [Export]
        public NodePath valueLabelPath = "MarginContainer2/ScrollContainer/Control/ConfLine/ValueLabel";

        [Export]
        public NodePath quitPath;

        bool open = false;
        TextureButton closeButton;
        HSlider renderSlider;
        Button quitBtn;

        Hud hud;
        Label valueLabel;
        GameManager gameManager;
        private SaveSystemNode saveSystemNode;

        public override void _Ready()
        {
            Visible = false;
            saveSystemNode = GetNode<SaveSystemNode>("/root/MainNode/SaveSystem");
            gameManager = GetNode<GameManager>("/root/GameManager");

            valueLabel = GetNode<Label>(valueLabelPath);

            hud = gameManager.hud;
            hud?.Connect("OnUpdateViewport3DSize", this, nameof(OnUpdateViewport3DScale));

            closeButton = GetNode<TextureButton>(closeButtonPath);
            closeButton?.Connect("button_up", this, nameof(Hide));

            renderSlider = GetNode<HSlider>(renderSliderPath);

            quitBtn = GetNode<Button>(quitPath);
            quitBtn?.Connect("button_up", this, nameof(Quit));

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

        public void Quit() {
            saveSystemNode.SaveGame(() => {
                GetTree().Quit();
            });
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
