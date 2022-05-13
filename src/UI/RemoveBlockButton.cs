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

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            this.Connect("button_up", this, nameof(OnClick));
            gameManager.placementController.Connect(nameof(PlacementController.OnChangeEditMode), this, nameof(OnChangeEditMode));
        }

        public void OnChangeEditMode(bool inEditMode) {
            Visible = inEditMode;
        }

        public void OnClick()
        {
            selected = !selected;
            gameManager.placementController.ChangeRemoveBlockFlag(selected);
        }

    }
}
