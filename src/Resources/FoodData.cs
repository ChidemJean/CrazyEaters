using Godot;
using System;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources 
{
    [RegisteredType(nameof(FoodData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class FoodData : BuyableEntityData
    {
        [Export]
        public int calories;
    }
}