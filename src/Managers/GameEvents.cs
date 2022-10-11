using Godot;
using System;

namespace CrazyEaters.Managers 
{
    public enum GameEvent { 
        UpdateCharacterStatus,
        FoodEatFinish
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
}
