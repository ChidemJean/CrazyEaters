using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources 
{

    [RegisteredType(nameof(FoodData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class FoodData : ProjectileData
    {
        [Export]
        public int calories;

        [Export]
        List<string> recipeItems = new List<string>(); // {id}-{quantity}
    }
}