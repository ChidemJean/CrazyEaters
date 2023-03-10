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
    public class AccessoryData
    {
        public string id;
    }

    [System.Serializable]
    public class CharacterData
    {
        public string name;
        public int age;
        public string createdAt;
        public string id;
        public List<AccessoryData> accessories;
    }
    
    [System.Serializable]
    public class BlockItemSave {
        public int id;
        public float posX;
        public float posY;
        public float posZ;
        public float rotX;
        public float rotY;
        public float rotZ;
        public BlockItemSave(int id, Vector3 pos, Vector3 rot)
        {
            this.id = id;
            this.posX = pos.x;
            this.posY = pos.y;
            this.posZ = pos.z;
            this.rotX = rot.x;
            this.rotY = rot.y;
            this.rotZ = rot.z;
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
        public string uuid = "default";
        public string launcherActiveID = "default";
        public string habitatID = "default";
        public string datetimeLastPlay;
        public CharacterData character;
        public List<BlockItemSave> blocks;
        public List<StatusCharacter> statusesEater = new List<StatusCharacter>();

        public bool debug = false;
        public HabitatGameData(){}
        public HabitatGameData(string habitatID, List<BlockItemSave> blocks, List<StatusCharacter> statusesEater = null)
        {
            this.habitatID = habitatID;
            this.blocks = blocks;
            this.statusesEater = statusesEater;
        }
        public override string ToString()
        {
            string statusesString = "";
            foreach (StatusCharacter status in statusesEater) {
                statusesString += $" ({status.id}) -> ({status.current}) ";
            }

            return $"[uuid: {uuid}] - [launcher: {launcherActiveID}] - [character: {character.id}|{character.age}] - [qtd blocos: {blocks?.Count}] - [statuses: {statusesString}]";
        }
    }

    [System.Serializable]
    public class HabitatsGameData : GameData
    {
        public List<HabitatGameData> habitats = new List<HabitatGameData>();
        public HabitatsGameData(){}
        public HabitatsGameData(List<HabitatGameData> habitats = null)
        {
            this.habitats = habitats;
        }
    }
}