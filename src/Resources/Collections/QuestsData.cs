namespace CrazyEaters.Resources
{
    using Godot;
    using Godot.Collections;
    using System;
    using System.Collections.Generic;

    public class QuestsData : Resource
    {
        [Export]
        public List<QuestData> quests = new List<QuestData>();

    }
}