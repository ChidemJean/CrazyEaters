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

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            this.Connect("button_up", this, nameof(OnClick));
        }

        public void OnClick()
        {
            selected = !selected;
            gameManager.placementController.ChangeEditMode(selected);
        }

    }
}
