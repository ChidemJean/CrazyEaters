namespace CrazyEaters.Resources
{
    using Godot;
    using System;
    using MonoCustomResourceRegistry;

    [RegisteredType(nameof(StatusCharacter), "res://assets/textures/UI/icons/icon_block.png", nameof(Resource))]
    public class StatusCharacter : Resource
    {
        public enum UIVisibility { Explicit, Implicit };

        [Export]
        public string name = "Status Character";

        [Export]
        public string key = "key";

        [Export]
        public int max = 100;

        [Export]
        public int curValue = 100;

        [Export]
        public float seconds = 0;
        
        [Export]
        public float curSeconds = 0;

        [Export]
        public int variation = 0;

        [Export]
        public int damage = 0;

        [Export]
        public float secondsForDamage = 0;

        [Export]
        public float curSecondsForDamage = 0;

        [Export]
        public GradientTexture fillGradient;

        [Export]
        public Texture icon;

        [Export]
        public UIVisibility uiVisibility = UIVisibility.Explicit;

        [Export]
        public bool acceptModifiers = true;

        [Export]
        public CharacterType characterType;
    }
}