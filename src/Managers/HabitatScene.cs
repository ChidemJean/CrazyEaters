namespace CrazyEaters.Managers
{
    using Godot;
    using System;
    using CrazyEaters.Controllers;

    public class HabitatScene : CEScene
    {
        [Export]
        NodePath cameraPath;
        [Export]
        NodePath gameViewportPath;
        [Export]
        NodePath placementControllerPath;

        public Camera camera;
        public Viewport gameViewport;
        public PlacementController placementController;

        public override void _Ready()
        {
            base._Ready();
            
            camera = GetNode<Camera>(cameraPath);
            gameViewport = GetNode<Viewport>(gameViewportPath);
            placementController = GetNode<PlacementController>(placementControllerPath);
        }
    }
}
