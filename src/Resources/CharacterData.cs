using Godot;
using System;
using System.Collections.Generic;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources
{
    [RegisteredType(nameof(CharacterData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class CharacterData : BuyableEntityData
    {
        [Export] public List<CharacterAgeData> agesData;
    }
}