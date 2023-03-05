using Godot;
using System;

namespace CrazyEaters.UI
{
    public class ItemStat : NinePatchRect
    {
        [Export] private NodePath iconPath;
        [Export] private NodePath namePath;
        [Export] private NodePath valuePath;
        [Export] private NodePath bonusPath;

        private TextureRect icon;
        private Label name;
        private Label valueItem;
        private Label bonus;

        public string Name {
            set {
                name.Text = value;
            }
        }
        public string Value {
            set {
                valueItem.Text = value;
            }
        }
        public string Bonus {
            set {
                bonus.Text = value;
                bonus.Visible = true;
            }
        }
        public Texture Icon {
            set {
                icon.Texture = value;
            }
        }

        public override void _Ready()
        {
            name = GetNode<Label>(namePath);
            valueItem = GetNode<Label>(valuePath);
            bonus = GetNode<Label>(bonusPath);
            icon = GetNode<TextureRect>(iconPath);
        }
    }
}