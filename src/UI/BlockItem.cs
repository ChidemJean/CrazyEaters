using Godot;
using System;
using CrazyEaters.Resources;

namespace CrazyEaters.UI
{

    public class BlockItem : PanelContainer
    {
        [Export]
        public NodePath thumbPath;
        public TextureRect thumb;

        public BlockData blockData;
        public bool selected = false;
        public BottomMenu bottomMenu;

        public override void _Ready()
        {
            thumb = GetNode<TextureRect>(thumbPath);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                InputEventMouseButton _event = (InputEventMouseButton) @event;

                if (_event.IsPressed() && _event.ButtonIndex == (int) ButtonList.Left) {
                    bool mouseInside = GetGlobalRect().HasPoint(_event.Position);
                    SetSelect(mouseInside);
                    if (mouseInside) bottomMenu.ChangeBlock(this);
                }
            }
        }

        public void SetBlockData(BlockData blockData)
        {
            this.blockData = blockData;
            thumb.Texture = this.blockData.thumb;
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