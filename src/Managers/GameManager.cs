namespace CrazyEaters.Managers 
{
    using Godot;
    using System;
    using CrazyEaters.Controllers;
    using CrazyEaters.Optimization;

    public class GameManager : Spatial
    {
        public Camera camera;
        public Viewport gameViewport;
        public PlacementController placementController;
        public Hud hud;
        public Godot.Object ResourceQueue;

        public override void _Ready()
        {
            camera = GetNode<Camera>("/root/MainNode/ViewportContainer/Viewport/HabitatScene/CameraPivot/Camera");
            gameViewport = GetNode<Viewport>("/root/MainNode/ViewportContainer/Viewport");
            placementController = GetNode<PlacementController>("/root/MainNode/ViewportContainer/Viewport/HabitatScene/CSGMain");
            hud = GetNode<Hud>("/root/MainNode");
        }

    }
}