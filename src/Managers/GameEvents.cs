using Godot;
using System;

namespace CrazyEaters.Managers 
{
    public enum GameEvent { 
        UpdateCharacterStatus,
        FoodEatFinish,
        GameModeChange,
        MenuBottomItemClick,
        ChangePanel,
        OpenQuestsPanel,
        CloseQuestsPanel,
        ChangeScene,
        InNight,
        InDay,
        LevelUpdate,
        XpUpdate,
        CoinsUpdate,
        BeansUpdate,
        SliderSelectorChange,
        OnInventoryOperationRequest,
    };

    public class InventoryOperationRequest {
        public string entityKey;
        public int newQtd;

        public InventoryOperationRequest(string entityKey, int newQtd)
        {
            this.entityKey = entityKey;
            this.newQtd = newQtd;
        }
    }

    public class CharacterStatusEventData {
        public string name;
        public int newValue;

        public CharacterStatusEventData(string name, int newValue)
        {
            this.name = name;
            this.newValue = newValue;
        }
    }

    public class ChangePanelEventData {
        public string key;
        public int index;

        public ChangePanelEventData(string key, int index)
        {
            this.key = key;
            this.index = index;
        }
    }
}
