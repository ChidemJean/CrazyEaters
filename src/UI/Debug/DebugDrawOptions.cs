namespace CrazyEaters.UI.Debug
{
    using Godot;
    using System;
    using CrazyEaters.Managers;

    public class DebugDrawOptions : OptionButton
    {
        GameManager gameManager;

        public override void _Ready()
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
            Connect("item_selected", this, nameof(OnItemSelected));
        }

        public void OnItemSelected(int index) {
            Viewport.DebugDrawEnum type = Viewport.DebugDrawEnum.Disabled;
            switch (index) {
                case 1:
                    type = Viewport.DebugDrawEnum.Unshaded;
                    break;
                case 2:
                    type = Viewport.DebugDrawEnum.Overdraw;
                    break;
            }
            gameManager.gameViewport.DebugDraw = type;
        }

    }
}