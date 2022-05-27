namespace CrazyEaters.Resources
{
    using Godot;
    using System;

    public class BlockData : Resource
    {
        [Export]
        public PackedScene prefab;

        [Export]
        public Vector3 gridSize = new Vector3(1,1,1);
        
        [Export]
        public int life = 100;

        [Export]
        public float price = 0;

    }
}