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
        public int max = 100;

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