using System.Collections;
using System.Collections.Generic;
using Godot;

namespace CrazyEaters.Save
{
    [System.Serializable]
    public class AccountData : GameData
    {
        public UserData userData;
        public List<UnblockedEntityData> unblockedEntities;
        public List<QuestData> quests;
    }

    [System.Serializable]
    public class UserData
    {
        public string nickname;
        public int level;
        public int xp;
        public int yellowCoins;
        public int jellyGems;
    }

    [System.Serializable]
    public class UnblockedEntityData
    {
        public string id;
        public string datetimeUnblocked;
    }

    [System.Serializable]
    public class QuestData
    {
        public string id;
        public string datetimeOpen;
        public string datetimeCompleted;
        public string duration;
    }
}