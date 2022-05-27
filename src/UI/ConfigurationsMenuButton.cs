namespace CrazyEaters.UI 
{
    using Godot;
    using System;

    public class ConfigurationsMenuButton : TextureButton
    {
        [Export]
        NodePath configurationsMenuPath = "";
        ConfigurationsMenu configurationsMenu = null;

        public override void _Ready()
        {
            if (configurationsMenuPath != "") {
                configurationsMenu = GetNode<ConfigurationsMenu>(configurationsMenuPath);
            }

            this.Connect("button_up", this, nameof(OnClick));
        }

        public void OnClick() {
            if (configurationsMenu != null) {
                configurationsMenu.Show();
            }
        }
    }
}
