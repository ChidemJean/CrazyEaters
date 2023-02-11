using Godot;
using System;

namespace CrazyEaters.UI.Generics 
{
    public class BlueButtonNinePatch : NinePatchRect
    {
        [Export]
        public Texture texturePressed;

        [Export]
        public Texture textureNormal;

        [Export]
        public float offsetYChildOnPressed = .07f;

        private Texture originalTexture;

        public override void _Ready()
        {
            Texture = textureNormal;
        }

        public void OnGuiInput(InputEvent @event) 
        {
            if (@event is InputEventMouseButton) {
                if (((InputEventMouseButton) @event).IsPressed()) {
                    PressedEffect();
                    return;
                }
                UnpressedEffect();
            }
        }

        public void PressedEffect() 
        {
            Texture = texturePressed;
            Control firstChild = GetChildOrNull<Control>(0);
            if (firstChild != null) {
                firstChild.RectPosition = new Vector2(firstChild.RectPosition.x, firstChild.RectPosition.y + RectSize.y * offsetYChildOnPressed);
            }
        }
        public void UnpressedEffect() 
        {
            Texture = textureNormal;
            Control firstChild = GetChildOrNull<Control>(0);
            if (firstChild != null) {
                firstChild.RectPosition = new Vector2(firstChild.RectPosition.x, firstChild.RectPosition.y - RectSize.y * offsetYChildOnPressed);
            }
        }
    }
}