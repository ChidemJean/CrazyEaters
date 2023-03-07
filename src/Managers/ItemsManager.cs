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
        [Export] public CustomRarity commonCustom;
        [Export] public CustomRarity uncommonCustom;
        [Export] public CustomRarity epicCustom;
        [Export] public CustomRarity superCustom;
        [Export] private BuyablesData allItems;
        [Export] private EntityTypes entityTypes;
        [Inject] private GameManager gameManager;
        [Inject] private SaveSystemNode saveSystemNode;

        [Signal] public delegate void InventoryUpdate();

        public EntityTypes Types {
            get {
                return entityTypes;
            }
        }

        public override void _Ready()
        {
            this.ResolveDependencies();
            saveSystemNode.LoadInventory(OnLoadInventory);
            saveSystemNode.LoadAccount(OnLoadAccount);
            gameManager.StartListening(GameEvent.OnInventoryOperationRequest, OnInventoryOperationRequest);
        }

        public void OnLoadInventory(InventoryData inventoryData)
        {
            if (inventoryData == null || inventoryData.items == null) return;
            foreach (var item in inventoryData.items) {
                BuyableEntityData resource = FindByKey(item.id);
                if (resource != null) {
                    resource._qtd = item.qtd;
                }
            }
        }

        public void OnInventoryOperationRequest(object param)
        {
            InventoryOperationRequest request = (InventoryOperationRequest) param;
            UpdateItemInventory(request.entityKey, request.newQtd);
        }

        public void UpdateItemInventory(string id, int modQtd)
        {
            foreach (BuyableEntityData item in allItems.buyables) {
                if (item.stackable && item.id == id) {
                    int newQtd = item._qtd + modQtd;
                    item._qtd = newQtd < 0 ? 0 : newQtd;
                }
            }
            SaveInventory();
        }

        public void SaveInventory()
        {
            List<ItemSave> itemSaves = new List<ItemSave>();
            foreach (BuyableEntityData item in allItems.buyables) {
                if (item.stackable && item._qtd > 0) {
                    itemSaves.Add(new ItemSave(item.id, item._qtd));
                }
            }
            InventoryData inventoryData = new InventoryData(itemSaves);
            saveSystemNode.SaveInventory(inventoryData);
            EmitSignal("InventoryUpdate");
        }

        public void OnLoadAccount(AccountData accountData)
        {
            if (accountData == null || accountData.unblockedEntities == null) return;
            foreach (var item in accountData.unblockedEntities) {
                BuyableEntityData resource = FindByKey(item.id);
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
        
        public BuyableEntityData FindByKey(string id)
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
            return FindByKey(id)?.prefab;
        }

        public List<BuyableEntityData> GetAll() 
        {
            return allItems.buyables;
        }

        public List<BuyableEntityData> GetInventory() 
        {
            return allItems.buyables.FindAll((item) => {
                return item._qtd > 0;
            }).Cast<BuyableEntityData>().ToList();
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
        public List<HabitatData> GetHabitats(bool unblocked = false) 
        {
            return allItems.buyables.FindAll((item) => {
                var isHabitat = item is CrazyEaters.Resources.HabitatData;
                var isUnblocked = !unblocked || (unblocked && (!item._blocked || item.unblockedByDefault));
                return isHabitat && isUnblocked;
            }).Cast<HabitatData>().ToList();
        }
        public List<ProjectileData> GetProjectiles() 
        {
            return allItems.buyables.FindAll((item) => {
                return item is ProjectileData;
            }).Cast<ProjectileData>().ToList();
        }
        public List<CrazyEaters.Resources.CharacterData> GetCharacters(bool unblocked = false) 
        {
            return allItems.buyables.FindAll((item) => {
                var isCharacter = item is CrazyEaters.Resources.CharacterData;
                var isUnblocked = !unblocked || (unblocked && (!item._blocked || item.unblockedByDefault));
                return isCharacter && isUnblocked;
            }).Cast<CrazyEaters.Resources.CharacterData>().ToList();
        }
        public List<CrazyEaters.Resources.LauncherData> GetLaunchers(bool unblocked = false) 
        {
            return allItems.buyables.FindAll((item) => {
                var isLauncher = item is CrazyEaters.Resources.LauncherData;
                var isUnblocked = !unblocked || (unblocked && (!item._blocked || item.unblockedByDefault));
                return isLauncher && isUnblocked;
            }).Cast<CrazyEaters.Resources.LauncherData>().ToList();
        }

        public CustomRarity GetCustomRarity(EntityRarity rarity)
        {
            switch (rarity) {
                case EntityRarity.Common:
                    return commonCustom;
                case EntityRarity.Uncommon:
                    return uncommonCustom;
                case EntityRarity.Epic:
                    return epicCustom;
                case EntityRarity.Super:
                    return superCustom;
            }
            return null;
        }

        public EntityType GetEntityType(string id)
        {
            string key = GetEntityTypeString(id);
            foreach (var type in entityTypes.types) {
                if (type.key == key) {
                    return type;
                }
            }
            return null;
        }

        public string GetEntityTypeString(string id)
        {
            EntityData entityData = FindByKey(id);
            string _key = entityData.GetType().Name.ToLower().Replace("data", "");
            return _key;
        }

        public EntityType GetEntityTypeByKey(string id)
        {
            foreach (var type in entityTypes.types) {
                if (type.key == id) {
                    return type;
                }
            }
            return null;
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.OnInventoryOperationRequest, OnInventoryOperationRequest);
        }
    }
}