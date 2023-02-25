namespace CrazyEaters.Managers
{
    using Godot;
    using System;
    using CrazyEaters.Controllers;
    using CrazyEaters.Save;
    using CrazyEaters.Resources;
    using CrazyEaters.DependencyInjection;


    public class HabitatScene : CEScene
    {
        [Export]
        NodePath main3DNodePath;
        [Export]
        NodePath cameraPath;
        [Export]
        NodePath gameViewportPath;
        [Export]
        NodePath placementControllerPath;
        [Export]
        public bool inEditMode = false;

        public Spatial main3DNode;
        public Camera camera;
        public Viewport gameViewport;
        public PlacementController placementController;
        [Export]
        public int currentBlockId = 3;


        public override void _Ready()
        {
            base._Ready();
            
            camera = GetNode<Camera>(cameraPath);
            gameViewport = GetNode<Viewport>(gameViewportPath);
            main3DNode = GetNode<Spatial>(main3DNodePath);
            placementController = GetNode<PlacementController>(placementControllerPath);

            gameManager.currentMainNode3D = main3DNode;
        }

        public BlockData RetrieveBlockDataFromId(int blockId)
        {
            BlockData data = null;
            // this.blocksData.blocks.TryGetValue(blockId + "", out data);
            return data;
        }
    }
}
