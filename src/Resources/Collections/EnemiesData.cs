namespace CrazyEaters.Resources
{
    using Godot;
    using Godot.Collections;
    using System;
    using System.Collections.Generic;

    public class EnemiesData : Resource
    {
        [Export]
        public List<EnemyData> enemies = new List<EnemyData>();

    }
}