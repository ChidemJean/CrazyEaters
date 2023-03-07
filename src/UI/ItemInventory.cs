using Godot;
using System;
using CrazyEaters.Resources;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;
using Godot.Collections;

namespace CrazyEaters.UI
{
    public class ItemInventory : Control
    {
        [Inject] private ItemsManager itemsManager;
        [Export] private EntityRarity rarity = EntityRarity.Common;
        [Export] public NodePath ctnRarityPath;
        [Export] public NodePath shineTextPath;
        [Export] public NodePath thumbTextPath;
        [Export] public NodePath labelQtdPath;
        [Export] public NodePath typeTextPath;
        [Export] private string entityKey;
        
        private Control ctnRarity;
        private TextureRect shineText;
        private TextureRect thumbText;
        private Label labelQtd;
        private TextureRect typeText;

        public string QtdText {
            set {
                labelQtd.Text = "x " + value;
            }
            get {
                return labelQtd.Text;
            }
        }
        public Texture Thumb {
            set {
                thumbText.Texture = value;
            }
        }
        public string EntityKey {
            set {
                entityKey = value;
                EntityType type = itemsManager.GetEntityType(value);
                if (type != null) typeText.Texture = type.icon;
            }
            get {
                return entityKey;
            }
        }
        public EntityRarity Rarity {
            set {
                rarity = value;
                UpdateRarityUI();
            }
            get {
                return rarity;
            }
        }
        public Action onReady;

        public override void _Ready()
        {
            this.ResolveDependencies();
            ctnRarity = GetNode<Control>(ctnRarityPath);
            shineText = GetNode<TextureRect>(shineTextPath);
            thumbText = GetNode<TextureRect>(thumbTextPath);
            labelQtd = GetNode<Label>(labelQtdPath);
            typeText = GetNode<TextureRect>(typeTextPath);

            onReady?.Invoke();

            UpdateRarityUI();
        }

        public void UpdateRarityUI()
        {
            int qtd = 1;
            Color nColor = new Color(1f,0.79f,0.4f,0.32f);
            Material _material = null;
            var texts = ctnRarity.GetChildren();
            
            CustomRarity rarityData = itemsManager.GetCustomRarity(rarity);
            nColor = rarityData.color;
            if (rarityData.material != null) _material = rarityData.material;

            switch (rarity) {
                case EntityRarity.Common:
                    break;
                case EntityRarity.Uncommon:
                    qtd = 2;
                    break;
                case EntityRarity.Epic:
                    qtd = 3;
                    break;
                case EntityRarity.Super:
                    qtd = 4;
                    break;
            }

            for (int i = 0; i < texts.Count; i++) {
                TextureRect _text = (TextureRect) texts[i];
                if (i < qtd) {
                    _text.Modulate = nColor;
                } else {
                    _text.Modulate = new Color(0,0,0,0.32f);
                }
                if (_material != null) {
                    _text.Material = _material;
                }
            }
        }
    }
}