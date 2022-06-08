using System.Collections;
using System.Collections.Generic;
using Godot;

namespace CrazyEaters.Save
{
    
    [System.Serializable]
    public class BlockItemSave {
        public int id;
        public float posX;
        public float posY;
        public float posZ;
        public BlockItemSave(int id, Vector3 pos)
        {
            this.id = id;
            this.posX = pos.x;
            this.posY = pos.y;
            this.posZ = pos.z;
        }
    }

    [System.Serializable]
    public class ChunkSave {
        public float posX;
        public float posY;
        public float posZ;
        public List<BlockItemSave> blocks;
        public ChunkSave(Vector3 pos, List<BlockItemSave> blocks)
        {
            this.posX = pos.x;
            this.posY = pos.y;
            this.posZ = pos.z;
            this.blocks = blocks;
        }
    }

    [System.Serializable]
    public class GameData
    {
        public string launcherActiveID = "default";
        public List<ChunkSave> chunks = new List<ChunkSave>();

        public bool debug = false;
        public GameData(){}
        public GameData(List<ChunkSave> chunks)
        {
            this.chunks = chunks;
        }
    }
}