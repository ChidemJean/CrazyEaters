namespace CrazyEaters.Resources
{
    using Godot;
    using Godot.Collections;
    using System;
    using System.Collections.Generic;

    public class BuyablesData : Resource
    {
        [Export]
        public List<BuyableEntityData> buyables = new List<BuyableEntityData>();

    }
}