using Godot;
using System;
using MonoCustomResourceRegistry;

namespace CrazyEaters.Resources 
{
    public enum CoinType
    {
        Money,
        YellowCoin,
        PurpleCoin
    }

    [RegisteredType(nameof(BuyableEntityData), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class BuyableEntityData : EntityData
    {
        [Export]
        public int price;
        [Export]
        public CoinType coinType = CoinType.YellowCoin;
    }
}