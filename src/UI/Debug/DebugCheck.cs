namespace CrazyEaters.UI.Debug
{
    using Godot;
    using System;
    using CrazyEaters.Managers;

    public class DebugCheck : CheckButton
    {
        GameManager gameManager;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            Connect("toggled", this, nameof(OnToggle));
        }

        public void OnToggle(bool pressed) {
            gameManager.SetInDebug(pressed);
        }
    }
}