namespace CrazyEaters.UI.Debug
{
    using Godot;
    using System;
    using CrazyEaters.Managers;

    public class DebugMenu : PanelContainer
    {

        GameManager gameManager;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            Visible = false;
            gameManager.Connect(nameof(GameManager.OnDebugModeChange), this, nameof(OnDebugChange));
        }

        public void OnDebugChange(bool inDebug) {
            Visible = inDebug;
        }

    }
}