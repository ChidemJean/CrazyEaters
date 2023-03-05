using Godot;
using System;
using CrazyEaters.Resources;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class LauncherItem : TextureRect
    {
        [Inject] public ItemsManager itemsManager;
        [Export] public EntityRarity rarity = EntityRarity.Common;
        [Export] private NodePath thumbPath;
        [Export] private NodePath selectedPath;
        [Export] private NodePath lockPath;
        [Export] private NodePath ctnRarityPath;
        [Export] private Color modulateThumbBlocked;
        [Export] private string key;
        [Export] private bool isSelected;

        private TextureRect thumb;
        private TextureRect selected;
        private TextureRect lockIcon;
        private Control ctnRarity;

        [Signal] public delegate void click(string key);

        public string Key {
            set {
                key = value;
            }
            get {
                return key;
            }
        }
        public bool IsSelected {
            set {
                isSelected = value;
                selected.Visible = isSelected;
            }
        }
        public Texture Thumb {
            set {
                thumb.Texture = value;
            }
        }
        public EntityRarity Rarity {
            set {
                rarity = value;
                UpdateRarityUI();
            }
        }

        public Action onReady;

        public override void _Ready()
        {
            this.ResolveDependencies();
            thumb = GetNode<TextureRect>(thumbPath);
            selected = GetNode<TextureRect>(selectedPath);
            lockIcon = GetNode<TextureRect>(lockPath);
            ctnRarity = GetNode<Control>(ctnRarityPath);

            if (onReady != null) onReady.Invoke();
            Connect("gui_input", this, nameof(OnItemGuiInput));

            UpdateRarityUI();
        }

        public void IsLocked(bool locked)
        {
            thumb.SelfModulate = locked ? modulateThumbBlocked : new Color(1,1,1,1);
            lockIcon.Visible = locked;
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
        
        public void OnItemGuiInput(InputEvent @event) 
        {
            if (!(@event is InputEventMouseButton)) return;
            InputEventMouseButton _event = (InputEventMouseButton) @event;
            if (_event.IsPressed()) return;
            EmitSignal("click", key);
        }
    }
}
