using Godot;
using System;
using System.Collections.Generic;

namespace CrazyEaters.Resources 
{
    public class EntityTypes : Resource
    {
        [Export] public List<EntityType> types = new List<EntityType>();
    }
}
