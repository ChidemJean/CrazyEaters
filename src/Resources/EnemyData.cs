using Godot;
using System;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources
{
    [RegisteredType(nameof(EnemyData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class EnemyData : EntityData
    {

    }
}