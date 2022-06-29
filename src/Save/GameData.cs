using System.Collections;
using System.Collections.Generic;
using Godot;

namespace CrazyEaters.Save
{
    [System.Serializable]
    public class GameData
    {
    }
    
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
    public class StatusCharacter {
        public string id;
        public int current;
        public StatusCharacter(string id, int current)
        {
            this.id = id;
            this.current = current;
        }
    }

    [System.Serializable]
    public class HabitatGameData : GameData
    {
        public string launcherActiveID = "default";
        public List<ChunkSave> chunks = new List<ChunkSave>();
        public List<StatusCharacter> statusesEater = new List<StatusCharacter>();

        public bool debug = false;
        public HabitatGameData(){}
        public HabitatGameData(List<ChunkSave> chunks, List<StatusCharacter> statusesEater = null)
        {
            this.chunks = chunks;
            this.statusesEater = statusesEater;
        }
    }
}