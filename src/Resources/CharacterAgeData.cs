using Godot;
using System;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources
{
    [RegisteredType(nameof(CharacterAgeData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class CharacterAgeData : Resource
    {
        [Export] public string name;
        [Export] public Texture icon;
        [Export] public string timeToEvolve;
        [Export] public PackedScene prefab;
        [Export] public StatusesCharacter statusesCharacter;
    }
}
