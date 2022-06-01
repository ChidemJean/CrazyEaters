namespace CrazyEaters.Sandbox
{
    using Godot;
    using System;
    using CrazyEaters.Entities;

    public class Chunk : Spatial
    {
        [Export]
        public Vector3 size;

        private Block[,,] chunkData;

        public override void _Ready()
        {
            
        }
    }
}
