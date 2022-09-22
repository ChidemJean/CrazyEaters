using Godot;
using System;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources 
{
    public enum UnblockCondition { Equal, Different, Larger, Smaller, BiggerOrEqual, LessOrEqual }
    public enum UnblockOperator { And, Or }

    [RegisteredType(nameof(EntityUnblockCondition), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class EntityUnblockCondition : Resource
    {
        [Export]
        public string identification;
        [Export]
        public UnblockCondition condition;
        [Export]
        public object value;
        [Export]
        public UnblockOperator operador;
    }
}