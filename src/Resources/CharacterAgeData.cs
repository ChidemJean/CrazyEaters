using Godot;
using System;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources
{
    [RegisteredType(nameof(CharacterAgeData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class CharacterAgeData : Resource
    {
        [Export] private string name;
        [Export] private Texture icon;
        [Export] private string timeToEvolve;
        [Export] private PackedScene prefab;
        [Export] private StatusesCharacter statusesCharacter;
    }
}
