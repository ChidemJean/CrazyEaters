using System.Collections;
using System.Collections.Generic;
using Godot;

namespace CrazyEaters.Save
{
    [System.Serializable]
    public class InventoryData : GameData
    {
        public List<ItemSave> items;
        public InventoryData(List<ItemSave> items)
        {
            this.items = items;
        }
    }

    [System.Serializable]
    public class ItemSave {
        public string id;
        public int qtd;
        public ItemSave(string id, int qtd)
        {
            this.id = id;
            this.qtd = qtd;
        }
    }
}