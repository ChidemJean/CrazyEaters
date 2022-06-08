namespace CrazyEaters.Managers 
{
    using Godot;
    using System;
    using CrazyEaters.Controllers;
    using CrazyEaters.Optimization;
    using CrazyEaters.Sandbox;
    using CrazyEaters.Save;
    using Godot.Collections;
    using System.Collections.Generic;

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
        public GameData gameData = null;

        [Signal]
        public delegate void OnDebugModeChange(bool inDebug);

        [Signal]
        public delegate void OnGameDataLoaded();

        public override void _Ready()
        {
            GDScript resourceQueue = (GDScript) GD.Load("res://resource_queue.gd");
            Godot.Object resourceQueueObj = (Godot.Object) resourceQueue.New();

            camera = GetNode<Camera>("/root/MainNode/SceneSwitcher/MainSceneHabitat/ViewportContainer/Viewport/HabitatScene/CameraPivot/Camera");
            gameViewport = GetNode<Viewport>("/root/MainNode/SceneSwitcher/MainSceneHabitat/ViewportContainer/Viewport");
            placementController = GetNode<PlacementController>("/root/MainNode/SceneSwitcher/MainSceneHabitat/ViewportContainer/Viewport/HabitatScene/CSGMain");

            hud = GetNode<Hud>("/root/MainNode");
        }

        public void SetInDebug(bool inDebug) {
            this.inDebug = inDebug;
            EmitSignal(nameof(OnDebugModeChange), this.inDebug);
        }

#region SAVE_SYSTEM
        public async void SaveGame(Action OnSaveEnd) {
            //TODO: check if data is equal before save?
            gameData = ToGameData();
            await SaveSystem.SaveGame(gameData);
            OnSaveEnd();
        }

        public async void LoadGame(Action<GameData> OnLoad) {
            gameData = await SaveSystem.LoadGame();
            OnLoad(gameData);
        }

        public GameData ToGameData() {
            List<ChunkSave> chunksData = new List<ChunkSave>();

            foreach(Vector3 chunkPosition in world.chunks.Keys) 
            {
                Chunk chunk = (Chunk) world.chunks[chunkPosition];
                List<BlockItemSave> blocksData = new List<BlockItemSave>();

                foreach(Vector3 blockPos in chunk.data.Keys)
                {
                    int blockId = (int) chunk.data[blockPos];
                    BlockItemSave blockData = new BlockItemSave(blockId, blockPos);
                    blocksData.Add(blockData);
                }

                ChunkSave chunkData = new ChunkSave(chunkPosition, blocksData);
                chunksData.Add(chunkData);
            }

            return new GameData(chunksData);
        }

        public Dictionary FromGameData(GameData _gameData) {
            if (_gameData == null) return null;

            Dictionary chunks = new Dictionary();

            foreach (ChunkSave chunkData in _gameData.chunks) {
                Vector3 chunkPosition = new Vector3(chunkData.posX, chunkData.posY, chunkData.posZ);

                Chunk chunk = new Chunk();
                chunk.chunkPosition = chunkPosition;

                Dictionary blocks = new Dictionary();
                foreach (BlockItemSave blockData in chunkData.blocks) {
                    blocks[new Vector3(blockData.posX, blockData.posY, blockData.posZ)] = blockData.id;
                }

                chunk.data = blocks;
                chunks[chunkPosition] = chunk;

            }

            return chunks;
        }
#endregion
    }
}