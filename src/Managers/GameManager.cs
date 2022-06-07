namespace CrazyEaters.Managers 
{
    using Godot;
    using System;
    using CrazyEaters.Controllers;
    using CrazyEaters.Optimization;
    using CrazyEaters.Sandbox;

    public class GameManager : Spatial
    {
        public enum InputMode { SCENE, UI };
        public Camera camera;
        public Viewport gameViewport;
        public PlacementController placementController;
        public Hud hud;
        public Godot.Object ResourceQueue;
        public InputMode inputMode = InputMode.SCENE;

        public Vector3 gravityVector = (Vector3) ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
        public int gravityMagnitude = Convert.ToInt32(ProjectSettings.GetSetting("physics/3d/default_gravity"));
        public bool inDebug = false;
        public CrazyEaters.Sandbox.World world;

        [Signal]
        public delegate void OnDebugModeChange(bool inDebug);

        public override void _Ready()
        {
            camera = GetNode<Camera>("/root/MainNode/ViewportContainer/Viewport/HabitatScene/CameraPivot/Camera");
            gameViewport = GetNode<Viewport>("/root/MainNode/ViewportContainer/Viewport");
            placementController = GetNode<PlacementController>("/root/MainNode/ViewportContainer/Viewport/HabitatScene/CSGMain");
            hud = GetNode<Hud>("/root/MainNode");
        }

        public void SetInDebug(bool inDebug) {
            this.inDebug = inDebug;
            EmitSignal(nameof(OnDebugModeChange), this.inDebug);
        }

    }
}