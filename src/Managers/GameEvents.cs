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
        OpenQuestsPanel
    };

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
