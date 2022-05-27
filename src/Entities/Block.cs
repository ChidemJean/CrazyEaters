namespace CrazyEaters.Entities 
{
    using Godot;
    using System;
    using CrazyEaters.Resources;

    public class Block : Spatial
    {
        [Export]
        public BlockData data;

        public override void _Ready()
        {
            
        }

    }
}
