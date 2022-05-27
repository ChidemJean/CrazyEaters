namespace CrazyEaters.Optimization 
{

    using Godot;
    using System;

    public class Hud : Control
    {
        [Export] private float scaleFactor = 1.0f;
        private Viewport viewport3d;
        public float currentScale;

        [Signal]
        delegate void OnUpdateViewport3DSize(string type, float scale);

        public override void _Ready()
        {
            this.currentScale = scaleFactor;
            this.viewport3d = GetNode<Viewport>("ViewportContainer/Viewport");
            this.viewport3d.GetTexture().Flags = ((uint)Texture.FlagsEnum.Filter);

            UpdateViewport3DSize(scaleFactor);

            GetViewport().Connect("size_changed", this, nameof(OnViewportSizeChanged));
        }

        public void OnViewportSizeChanged()
        {
            UpdateViewport3DSize(this.currentScale);
        }

        public void UpdateViewport3DSize(float value, string type = "LOAD")
        {
            float scaleFactorClamped = Mathf.Clamp(value, 0.2f, 1f);
            this.viewport3d.Size = GetViewport().Size * scaleFactorClamped;
            this.currentScale = scaleFactorClamped;

            EmitSignal("OnUpdateViewport3DSize", type, this.currentScale);
        }
    }
}