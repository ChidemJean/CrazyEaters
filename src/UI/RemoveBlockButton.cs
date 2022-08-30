namespace CrazyEaters.UI
{
    using CrazyEaters.Managers;
    using CrazyEaters.Controllers;
    using Godot;
    using System;

    public class RemoveBlockButton : TextureButton
    {

        private bool selected = false;
        private GameManager gameManager;
        private SceneSwitcher sceneSwitcher;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            sceneSwitcher = GetNode<SceneSwitcher>("/root/MainNode/SceneSwitcher");
            this.Connect("button_up", this, nameof(OnClick));
            // ((HabitatScene)sceneSwitcher.currentScene).placementController.Connect(nameof(PlacementController.OnChangeEditMode), this, nameof(OnChangeEditMode));
        }

        public override void _Process(float delta)
        {
            if (((HabitatScene)sceneSwitcher.currentScene) != null) {
                OnChangeEditMode(((HabitatScene)sceneSwitcher.currentScene).inEditMode);
            }
        }

        public void OnChangeEditMode(bool inEditMode) {
            Visible = inEditMode;
        }

        public void OnClick()
        {
            selected = !selected;
            ((HabitatScene)sceneSwitcher.currentScene).placementController.ChangeRemoveBlockFlag(selected);
        }

    }
}
