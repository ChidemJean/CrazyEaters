using Godot;
using System;
using MonoCustomResourceRegistry;
using Godot.Collections;

namespace CrazyEaters.Resources 
{
    public enum EntityRarity { Common, Uncommon, Epic, Super }

    [RegisteredType(nameof(EntityData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class EntityData : Resource
    {
        [Export]
        public string id;

        [Export]
        public string name;
        
        [Export]
        public PackedScene prefab;

        [Export(PropertyHint.MultilineText)]
        public string description;

        [Export]
        public Texture thumb;

        [Export]
        public bool blocked;

        [Export]
        public bool unblockedByDefault;
        [Export]
        public bool stackable;
        [Export]
        public int maxStack;
        [Export]
        public EntityRarity rarity;

        [Export]
        public Dictionary<string, EntityUnblockCondition> conditionsToUnblock = new Dictionary<string, EntityUnblockCondition>();

        public int _qtd;
        public bool _blocked;
    }
}