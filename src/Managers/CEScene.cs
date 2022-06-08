namespace CrazyEaters.Managers
{
    using Godot;
    using System;
    using CrazyEaters.Managers;

    public class CEScene : Control
    {
        [Export]
        public NodePath viewport3dPath;

        public Viewport viewport3d = null;

        public GameManager gameManager;

        public override void _Ready()
        {
            viewport3d = GetNodeOrNull<Viewport>(viewport3dPath);
            gameManager = GetNode<GameManager>("/root/GameManager");
        }
    }
}
