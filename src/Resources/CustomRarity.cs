using Godot;
using System;
using MonoCustomResourceRegistry;
using Godot.Collections;

namespace CrazyEaters.Resources 
{
    public class CustomRarity : Resource {
        [Export] public string name;
        [Export] public Color color;
        [Export] public Material material;
    }
}