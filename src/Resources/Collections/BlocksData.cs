namespace CrazyEaters.Resources
{
    using Godot;
    using Godot.Collections;
    using System;

    public class BlocksData : Resource
    {
        [Export]
        public Dictionary<string, BlockData> blocks = new Dictionary<string, BlockData>();

    }
}