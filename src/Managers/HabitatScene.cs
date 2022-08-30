namespace CrazyEaters.Managers
{
    using Godot;
    using System;
    using CrazyEaters.Controllers;
    using CrazyEaters.Save;
    using CrazyEaters.Resources;


    public class HabitatScene : CEScene
    {
        [Export]
        NodePath cameraPath;
        [Export]
        NodePath gameViewportPath;
        [Export]
        NodePath placementControllerPath;
        [Export]
        public bool inEditMode = false;
        [Export]
        BlocksData blocksData;

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
            placementController = GetNode<PlacementController>(placementControllerPath);
        }

        public BlocksData GetBlocksData()
        {
            return this.blocksData;
        }

        public BlockData RetrieveBlockDataFromId(int blockId)
        {
            BlockData data = null;
            this.blocksData.blocks.TryGetValue(blockId + "", out data);
            return data;
        }
    }
}
