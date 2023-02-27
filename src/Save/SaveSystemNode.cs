namespace CrazyEaters.Save 
{
    using Godot;
    using System;
    using CrazyEaters.Controllers;
    using CrazyEaters.Optimization;
    using CrazyEaters.Sandbox;
    using CrazyEaters.Save;
    using CrazyEaters.Managers;
    using Godot.Collections;
    using System.Collections.Generic;

    public class SaveSystemNode : Node
    {

        public GameManager gameManager;

        public override void _Ready() {
            gameManager = GetNode<GameManager>("/root/GameManager");
        }

        public async void SaveAccount(AccountData accountData, Action OnSaveEnd = null) {
            await SaveSystem.SaveUserData(accountData);
            if (OnSaveEnd != null) OnSaveEnd();
        }

        public async void LoadAccount(Action<AccountData> OnLoad) {
            var data = await SaveSystem.LoadUserData();
            OnLoad(data);
        }

        public async void SaveHabitats(HabitatsGameData habitatsGameData, Action OnSaveEnd = null) {
            await SaveSystem.SaveHabitatsData(habitatsGameData);
            if (OnSaveEnd != null) OnSaveEnd();
        }

        public async void LoadHabitats(Action<HabitatsGameData> OnLoad) {
            var data = await SaveSystem.LoadHabitatsData();
            OnLoad(data);
        }

        public async void SaveInventory(InventoryData inventoryData, Action OnSaveEnd = null) {
            await SaveSystem.SaveInventoryData(inventoryData);
            if (OnSaveEnd != null) OnSaveEnd();
        }

        public async void LoadInventory(Action<InventoryData> OnLoad) {
            var data = await SaveSystem.LoadInventoryData();
            OnLoad(data);
        }

        public async void SaveMultiplayer(MultiplayerData multiplayerData, Action OnSaveEnd = null) {
            await SaveSystem.SaveMultiplayerData(multiplayerData);
            if (OnSaveEnd != null) OnSaveEnd();
        }

        public async void LoadMultiplayer(Action<MultiplayerData> OnLoad) {
            var data = await SaveSystem.LoadMultiplayerData();
            OnLoad(data);
        }

        public GameData ToGameData() {
            // List<ChunkSave> chunksData = new List<ChunkSave>();

            // foreach(Vector3 chunkPosition in gameManager.world.chunks.Keys) 
            // {
            //     Chunk chunk = (Chunk) gameManager.world.chunks[chunkPosition];
            //     List<BlockItemSave> blocksData = new List<BlockItemSave>();

            //     foreach(Vector3 blockPos in chunk.data.Keys)
            //     {
            //         int blockId = (int) chunk.data[blockPos];
            //         if (blockId != 0) {
            //             BlockItemSave blockData = new BlockItemSave(blockId, blockPos);
            //             blocksData.Add(blockData);
            //         }
            //     }

            //     ChunkSave chunkData = new ChunkSave(chunkPosition, blocksData);
            //     chunksData.Add(chunkData);
            // }

            return new HabitatGameData();
        }

        public Dictionary FromGameData(HabitatGameData _gameData) {
            // if (_gameData == null) return null;

            // Dictionary chunks = new Dictionary();

            // foreach (ChunkSave chunkData in _gameData.chunks) {
            //     Vector3 chunkPosition = new Vector3(chunkData.posX, chunkData.posY, chunkData.posZ);

            //     Chunk chunk = new Chunk();
            //     chunk.chunkPosition = chunkPosition;

            //     Dictionary blocks = new Dictionary();
            //     foreach (BlockItemSave blockData in chunkData.blocks) {
            //         blocks[new Vector3(blockData.posX, blockData.posY, blockData.posZ)] = blockData.id;
            //     }

            //     chunk.data = blocks;
            //     chunks[chunkPosition] = chunk;

            // }

            return null;
        }
    }
}