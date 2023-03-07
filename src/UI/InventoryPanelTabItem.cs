using Godot;
using System;
using System.Collections.Generic;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Resources;

namespace CrazyEaters.UI
{
    public class InventoryPanelTabItem : PanelTabItem
    {
        [Inject] private ItemsManager itemsManager;
        [Export] private NodePath ctnItemsPath;
        [Export] private PackedScene itemPrefab;
        [Export] private NodePath categoriesTabButtonsPath;
        [Export] private PackedScene categoryBtnPrefab;
        [Export] private List<string> categoriesSelected = new List<string>();

        private Control ctnItems;
        private Control categoriesTabButtons;

        public override void _Ready()
        {
            this.ResolveDependencies();
            base._Ready();
            ctnItems = GetNode<Control>(ctnItemsPath);
            categoriesTabButtons = GetNode<Control>(categoriesTabButtonsPath);
            itemsManager.Connect("InventoryUpdate", this, nameof(OnInventoryUpdate));

            CreateCategoriesButtons();
            CreateItems();
            UpdateTypesBtns();
        }

        public void CreateCategoriesButtons()
        {
            var types = itemsManager.Types;
            foreach (var type in types.types) {
                var newType = categoryBtnPrefab.Instance<CategoryTabButton>();
                newType.TextureNormal = type.icon;
                newType.Name = $"{type.key}";
                newType.Connect("click", this, nameof(OnCategoryClick));
                categoriesTabButtons.AddChild(newType);
            }
        }

        public void OnCategoryClick(string name)
        {
            if (categoriesSelected.Contains(name)) {
                if (categoriesSelected.Count == 1) {
                    //TODO: fazer sistema de notificação e avisar que não é possível selecionar nenhum tipo
                    return;
                }
                categoriesSelected.Remove(name);
            } else {
                categoriesSelected.Add(name);
            }
            UpdateTypesBtns();
        }

        public void CreateItems()
        {
            var items = itemsManager.GetInventory();
            foreach (var item in items) {
                var newItem = itemPrefab.Instance<ItemInventory>();
                newItem.onReady = () => {
                    newItem.QtdText = item._qtd.ToString();
                    newItem.Thumb = item.thumb;
                    newItem.EntityKey = item.id;
                    newItem.Rarity = item.rarity;
                };
                ctnItems.AddChild(newItem);
            }
        }

        public void CleanItems()
        {
            foreach (Node item in ctnItems.GetChildren()) {
                ctnItems.RemoveChild(item);
                item.QueueFree();
            }
        }

        public void UpdateTypesBtns() 
        {
            foreach (Node typeBtn in categoriesTabButtons.GetChildren()) {
                EntityType type = itemsManager.GetEntityTypeByKey(typeBtn.Name);
                ((TextureButton) typeBtn).TextureNormal = (categoriesSelected.Contains(typeBtn.Name)) ? type.iconColored : type.icon;
            }
            FilterItems();
        }

        public void FilterItems()
        {
            foreach (Node item in ctnItems.GetChildren()) {
                ItemInventory itemInv = (ItemInventory) item;
                string type = itemsManager.GetEntityTypeString(itemInv.EntityKey);
                itemInv.Visible = categoriesSelected.Contains(type);
            }
        }

        public void OnInventoryUpdate()
        {
            CleanItems();
            CreateItems();
            UpdateTypesBtns();
        }
    }
}