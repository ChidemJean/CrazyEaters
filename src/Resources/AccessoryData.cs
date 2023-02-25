using Godot;
using System;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources
{
    [RegisteredType(nameof(AccessoryData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class AccessoryData : BuyableEntityData
    {

    }
}