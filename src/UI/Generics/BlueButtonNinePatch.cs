using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI.Generics 
{
    public class BlueButtonNinePatch : NinePatchRect
    {
        [Inject]
        AudioStreamManager audioStreamManager;

        [Export]
        public Texture texturePressed;

        [Export]
        public Texture textureNormal;

        [Export]
        public Texture textureDisabledPressed;

        [Export]
        public Texture textureDisabledNormal;

        [Export]
        public float offsetYChildOnPressed = .07f;

        [Export]
        public bool emitSound = true;

        [Export]
        public string soundKey = "button_click";

        [Export]
        public bool IsDisabled {
            get { return this.isDisabled; }
            set {
                this.isDisabled = value;
                Texture = this.isDisabled ? textureDisabledNormal : textureNormal;
                SelfModulate = new Color(SelfModulate.r, SelfModulate.g, SelfModulate.b, this.isDisabled ? .2f : 1f);
            }
        }
        private bool isDisabled = false;

        [Export]
        public bool IsShinig {
            get { return this.isShinig; }
            set {
                this.isShinig = value;
                UseParentMaterial = !this.isShinig;
            }
        }
        private bool isShinig = false;

        private Texture originalTexture;

        [Signal] public delegate void click(bool pressed);

        public override void _Ready()
        {
            this.ResolveDependencies();
            Texture = isDisabled ? textureDisabledNormal : textureNormal;
            UseParentMaterial = !this.isShinig;
        }

        public void OnGuiInput(InputEvent @event) 
        {
            if (@event is InputEventMouseButton) {
                if (((InputEventMouseButton) @event).ButtonIndex != (int) ButtonList.Left) return;
                if (((InputEventMouseButton) @event).IsPressed()) {
                    PressedEffect();
                    EmitSignal("click", true);
                    return;
                }
                UnpressedEffect();
                EmitSignal("click", false);
            }
        }

        public void PressedEffect() 
        {
            Texture = isDisabled ? textureDisabledPressed : texturePressed;
            Control firstChild = GetChildOrNull<Control>(0);
            if (firstChild != null) {
                firstChild.RectPosition = new Vector2(firstChild.RectPosition.x, firstChild.RectPosition.y + RectSize.y * offsetYChildOnPressed);
            }
        }
        public void UnpressedEffect() 
        {
            if (emitSound) audioStreamManager.Play(soundKey);

            Texture = isDisabled ? textureDisabledNormal : textureNormal;
            Control firstChild = GetChildOrNull<Control>(0);
            if (firstChild != null) {
                firstChild.RectPosition = new Vector2(firstChild.RectPosition.x, firstChild.RectPosition.y - RectSize.y * offsetYChildOnPressed);
            }
        }
    }
}