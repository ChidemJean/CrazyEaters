using Godot;
using System;

namespace CrazyEaters.UI
{

    public class SaveDot : Control
    {
        [Export] private Texture underTexture;
        [Export] private Texture progressTexture;
        [Export] private Texture underSelectedTexture;
        [Export] private Texture progressSelectedTexture;
        [Export] private NodePath dotTexturePath;
        [Export] private NodePath labelPath;
        [Export] private Vector2 minSizeSelected;
        [Export] private Color modulateSelected;
        [Export] private Color _modulate;

        private Vector2 minSize;
        private string idx;
        private TextureProgress dotTexture;
        private Label label;
        private string saveUUID;
        private bool isSelected;
        public Action onReady;
        public string Idx {
            get {
                return idx;
            }
            set {
                idx = value;
                label.Text = idx;
            }
        }
        public bool IsSelected {
            get {
                return isSelected;
            }
            set {
                if (value) {
                    Select();
                } else {
                    Deselect();
                }
                isSelected = value;
            }
        }
        public string SaveUUID {
            get {
                return saveUUID;
            }
            set {
                saveUUID = value;
            }
        }

        [Signal] public delegate void click(string idx);

        public override void _Ready()
        {
            dotTexture = GetNode<TextureProgress>(dotTexturePath);
            label = GetNode<Label>(labelPath);
            minSize = dotTexture.RectMinSize;
            Connect("gui_input", this, nameof(OnGuiInput));
            if (onReady != null) onReady.Invoke();
        }

        public void Select()
        {
            if (isSelected) return;
            label.Visible = true;
            dotTexture.TextureUnder = underSelectedTexture;
            dotTexture.TextureProgress_ = progressSelectedTexture;
            dotTexture.SelfModulate = modulateSelected;
            UpdateSizes(minSizeSelected);
        }
        public void Deselect()
        {
            if (!isSelected) return;
            label.Visible = false;
            dotTexture.TextureUnder = underTexture;
            dotTexture.TextureProgress_ = progressTexture;
            dotTexture.SelfModulate = _modulate;
            UpdateSizes(minSize);
        }

        public void UpdateSizes(Vector2 size) {
            dotTexture.RectMinSize = size;
            dotTexture.RectSize = size;
            RectMinSize = new Vector2(size.x, RectMinSize.y);
            RectSize = new Vector2(size.x, RectMinSize.y);
        }

        public void OnGuiInput(InputEvent @event) 
        {
            if (@event is InputEventMouseButton) {
                if (((InputEventMouseButton) @event).ButtonIndex != (int) ButtonList.Left) return;
                if (((InputEventMouseButton) @event).IsPressed()) {
                    EmitSignal("click", idx);
                    return;
                }
            }
        }
    }
}