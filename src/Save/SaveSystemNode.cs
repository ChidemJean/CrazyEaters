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


#region SAVE_SYSTEM
        public async void SaveGame(Action OnSaveEnd) {
            //TODO: check if data is equal before save?
            gameManager.gameData = ToGameData();
            await SaveSystem.SaveGame(gameManager.gameData);
            OnSaveEnd();
        }

        public async void LoadGame(Action<GameData> OnLoad) {
            gameManager.gameData = await SaveSystem.LoadGame();
            OnLoad(gameManager.gameData);
        }

        public GameData ToGameData() {
            List<ChunkSave> chunksData = new List<ChunkSave>();

            foreach(Vector3 chunkPosition in gameManager.world.chunks.Keys) 
            {
                Chunk chunk = (Chunk) gameManager.world.chunks[chunkPosition];
                List<BlockItemSave> blocksData = new List<BlockItemSave>();

                foreach(Vector3 blockPos in chunk.data.Keys)
                {
                    int blockId = (int) chunk.data[blockPos];
                    if (blockId != 0) {
                        BlockItemSave blockData = new BlockItemSave(blockId, blockPos);
                        blocksData.Add(blockData);
                    }
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