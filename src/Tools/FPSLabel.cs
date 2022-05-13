namespace CrazyEaters.UI
{
    using Godot;
    using System;

    public class FPSLabel : Label
    {
        public override void _Process(float delta)
        {
            this.Text = "FPS: " + Engine.GetFramesPerSecond();
        }
    }
}