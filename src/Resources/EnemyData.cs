using Godot;
using System;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources
{
    [RegisteredType(nameof(EnemyData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class EnemyData : EntityData
    {
        [Export]
        public int maxLife = 100;

        [Export]
        public int curLife = 100;

        [Export]
        public int damage = 0;

        [Export]
        public float secondsForDamage = 0;

        [Export]
        public float curSecondsForDamage = 0;
    }
}