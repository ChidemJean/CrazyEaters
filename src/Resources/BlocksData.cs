namespace CrazyEaters.Resources
{
    using Godot;
    using Godot.Collections;
    using System;

    public class BlocksData : Resource
    {
        [Export]
        public Dictionary<string, PackedScene> blocks = new Dictionary<string, PackedScene>();

    }
}