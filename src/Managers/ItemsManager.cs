using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using CrazyEaters.Resources;
using CrazyEaters.Save;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.Managers
{
    public class ItemsManager : Node
    {
        [Export] private BuyablesData allItems;
        [Inject] private SaveSystemNode saveSystemNode;

        public override void _Ready()
        {
            this.ResolveDependencies();
            saveSystemNode.LoadInventory(OnLoadInventory);
            saveSystemNode.LoadAccount(OnLoadAccount);
        }

        public void OnLoadInventory(InventoryData inventoryData)
        {
            if (inventoryData == null || inventoryData.items == null) return;
            foreach (var item in inventoryData.items) {
                BuyableEntityData resource = Find(item.id);
                if (resource != null) {
                    resource._qtd = item.qtd;
                }
            }
        }

        public void OnLoadAccount(AccountData accountData)
        {
            if (accountData == null || accountData.unblockedEntities == null) return;
            foreach (var item in accountData.unblockedEntities) {
                BuyableEntityData resource = Find(item.id);
                if (resource != null) {
                    resource._blocked = false;
                }
            }
        }

        public BuyableEntityData FindUnblocked(string id)
        {
            foreach (BuyableEntityData item in allItems.buyables) {
                if (item.id == id && (!item.blocked || item.unblockedByDefault)) {
                    return item;
                }
            }
            return null;
        }
        
        public BuyableEntityData Find(string id)
        {
            foreach (BuyableEntityData item in allItems.buyables) {
                if (item.id == id) {
                    return item;
                }
            }
            return null;
        }

        public PackedScene GetPrefab(string id)
        {
            return Find(id)?.prefab;
        }

        public List<BlockData> GetBlocks() 
        {
            return allItems.buyables.FindAll((item) => {
                return item is BlockData;
            }).Cast<BlockData>().ToList();
        }
        public List<FoodData> GetFoods() 
        {
            return allItems.buyables.FindAll((item) => {
                return item is FoodData;
            }).Cast<FoodData>().ToList();
        }
        public List<HabitatData> GetHabitats() 
        {
            return allItems.buyables.FindAll((item) => {
                return item is HabitatData;
            }).Cast<HabitatData>().ToList();
        }
        public List<ProjectileData> GetProjectiles() 
        {
            return allItems.buyables.FindAll((item) => {
                return item is ProjectileData;
            }).Cast<ProjectileData>().ToList();
        }
        public List<CrazyEaters.Resources.CharacterData> GetCharacters() 
        {
            return allItems.buyables.FindAll((item) => {
                return item is CrazyEaters.Resources.CharacterData;
            }).Cast<CrazyEaters.Resources.CharacterData>().ToList();
        }
    }
}