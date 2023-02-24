namespace CrazyEaters.Resources
{
    using Godot;
    using System;
    using MonoCustomResourceRegistry;
    using CrazyEaters.Resources;

    [RegisteredType(nameof(BlockData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class BlockData : BuyableEntityData
    {
        public enum BlockType { Unique, Group };

        [Export]
        public Vector3 gridSize = new Vector3(1,1,1);
        
        [Export]
        public int life = 100;

        [Export]
        public float price = 0;

        [Export]
        public BlockType blockType = BlockType.Unique;

        [Export]
        public bool solid = false;

    }
}