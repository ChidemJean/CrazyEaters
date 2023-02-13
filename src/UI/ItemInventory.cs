using Godot;
using System;
using CrazyEaters.Resources;
using Godot.Collections;

namespace CrazyEaters.UI
{
    public class ItemInventory : Control
    {
        [Export]
        public EntityRarity rarity = EntityRarity.Common;
        [Export]
        public NodePath ctnRarityPath;
        [Export]
        public NodePath shineTextPath;
        [Export]
        public NodePath thumbTextPath;
        [Export]
        public NodePath labelQtdPath;
        [Export]
        public NodePath typeTextPath;
        [Export]
        public Material superMaterial;

        private Control ctnRarity;
        private TextureRect shineText;
        private TextureRect thumbText;
        private Label labelQtd;
        private TextureRect typeText;


        public override void _Ready()
        {
            ctnRarity = GetNode<Control>(ctnRarityPath);
            shineText = GetNode<TextureRect>(shineTextPath);
            thumbText = GetNode<TextureRect>(thumbTextPath);
            labelQtd = GetNode<Label>(labelQtdPath);
            typeText = GetNode<TextureRect>(typeTextPath);

            shineText.Material = superMaterial;
            labelQtd.Material = superMaterial;
            typeText.Material = superMaterial;

            UpdateRarityUI();
        }

        public void UpdateRarityUI()
        {
            int qtd = 1;
            Color nColor = new Color(1f,0.79f,0.4f,0.32f);
            var texts = ctnRarity.GetChildren();

            switch (rarity) {
                case EntityRarity.Common:
                    break;
                case EntityRarity.Uncommon:
                    qtd = 2;
                    nColor = new Color(0,1f,0,1f);
                    break;
                case EntityRarity.Epic:
                    qtd = 3;
                    nColor = new Color(0.55f,0f,1f,1f);
                    break;
                case EntityRarity.Super:
                    qtd = 4;
                    nColor = new Color(1f,1f,1f,1f);
                    break;
            }

            if (rarity == EntityRarity.Super) {
                shineText.UseParentMaterial = false;
                typeText.UseParentMaterial = false;
                labelQtd.UseParentMaterial = false;
                shineText.SelfModulate = new Color(1f,1f,1f,1f);
            } else {
                shineText.UseParentMaterial = true;
                typeText.UseParentMaterial = true;
                labelQtd.UseParentMaterial = true;
            }

            for (int i = 0; i < texts.Count; i++) {
                TextureRect _text = (TextureRect) texts[i];
                if (i < qtd) {
                    _text.Modulate = nColor;
                } else {
                    _text.Modulate = new Color(0,0,0,0.32f);
                }
                if (rarity == EntityRarity.Super) {
                    _text.UseParentMaterial = false;
                }
                _text.Material = superMaterial;
            }
        }
    }
}