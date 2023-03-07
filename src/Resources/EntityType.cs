using Godot;
using System;
using MonoCustomResourceRegistry;
using Godot.Collections;

namespace CrazyEaters.Resources 
{
    public class EntityType : Resource {
        [Export] public string key;
        [Export] public string name;
        [Export] public Texture icon;
        [Export] public Texture iconColored;
    }
}