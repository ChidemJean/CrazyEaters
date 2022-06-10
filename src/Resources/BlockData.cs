namespace CrazyEaters.Resources
{
    using Godot;
    using System;

    public class BlockData : Resource
    {
        public enum BlockType { Ground, Air, Water, Stone, Wood, Custom };

        [Export]
        public string id;
        [Export]
        public PackedScene prefab;

        [Export]
        public Vector3 gridSize = new Vector3(1,1,1);
        
        [Export]
        public int life = 100;

        [Export]
        public float price = 0;

        [Export]
        public BlockType blockType = BlockType.Air;

        [Export]
        public bool solid = false;

        [Export]
        public Texture thumb;

    }
}