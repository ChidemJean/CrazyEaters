using Godot;
using System;

namespace CrazyEaters.Managers
{
    public class GameCreateScene : CEScene
    {
        [Export] private NodePath closeBtnPath;

        private TextureButton closeBtn;

        public override void _Ready()
        {
            closeBtn = GetNode<TextureButton>(closeBtnPath);
            closeBtn.Connect("button_up", this, nameof(OnCloseClick));
        }

        public void OnCloseClick()
        {
            gameManager.TriggerEvent(GameEvent.ChangeScene, "home");
        }
    }
}