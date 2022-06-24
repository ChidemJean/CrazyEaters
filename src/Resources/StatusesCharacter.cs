namespace CrazyEaters.Resources
{
    using Godot;
    using Godot.Collections;
    using System;

    public class StatusesCharacter : Resource
    {
        [Export]
        public Dictionary<string, StatusCharacter> statuses = new Dictionary<string, StatusCharacter>();
    }
}