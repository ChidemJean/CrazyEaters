namespace CrazyEaters.UI
{
    using CrazyEaters.Managers;
    using CrazyEaters.Controllers;
    using Godot;
    using System;

    public class EditModeButton : TextureButton
    {

        private bool selected = false;
        private GameManager gameManager;
        private SceneSwitcher sceneSwitcher;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            sceneSwitcher = GetNode<SceneSwitcher>("/root/MainNode/SceneSwitcher");
            this.Connect("button_up", this, nameof(OnClick));
        }

        public void OnClick()
        {
            selected = !selected;
            ((HabitatScene)sceneSwitcher.currentScene).placementController.ChangeEditMode(selected);
        }

    }
}
