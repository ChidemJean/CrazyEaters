using System.Collections;
using System.Collections.Generic;
using Godot;

namespace CrazyEaters.Save
{
    [System.Serializable]
    public class MultiplayerData : GameData
    {
        public List<Friend> friends;
        public List<Invitation> invitations;
    }

    [System.Serializable]
    public class Friend
    {
        public string nickname;
        public string datetime;
    }

    [System.Serializable]
    public class Invitation
    {
        public string nickname;
        public int type;
        public string datetime;
        public string description;
    }
}