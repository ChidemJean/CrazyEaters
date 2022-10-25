using Godot;
using System;
using CrazyEaters.Resources;

namespace CrazyEaters.UI
{

    public class InventoryItem : PanelContainer
    {
        [Export]
        public NodePath thumbPath;
        public TextureRect thumb;

        public BuyableEntityData buyableEntityData;
        public bool selected = false;
        public BottomMenu bottomMenu;

        public override void _Ready()
        {
            thumb = GetNode<TextureRect>(thumbPath);
        }

        public override void _Input(InputEvent @event)
        {
        }

        public void SetBuyableData(BuyableEntityData data)
        {
            this.buyableEntityData = data;
            if (this.buyableEntityData.thumb != null) thumb.Texture = this.buyableEntityData.thumb;
        }

        public void SetSelect(bool selected) 
        {
            this.selected = selected;
            //
            StyleBoxFlat styleBox = (StyleBoxFlat) GetStylebox("block_item").Duplicate();
            Color color = styleBox.BorderColor;
            color.a = this.selected ? 1 : color.a;

            styleBox.BorderColor = color;

            AddStyleboxOverride("panel", styleBox);
        }

    }
}