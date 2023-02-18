using Godot;
using System;

namespace CrazyEaters.Debug 
{
    public class DevControls : Node
    {

        public override void _Ready()
        {
            string platform = OS.GetName();
            if (platform == "Windows") {
                ResizeWindow();
            }
        }

        public void ResizeWindow()
        {
            Vector2 screenSize = OS.GetScreenSize();
            Vector2 currentWindowSize = OS.WindowSize;
            Vector2 realWindowSize = OS.GetRealWindowSize();
            float aspectRatio = (currentWindowSize.y - 50f) / 1920;
            float newWidth = currentWindowSize.x * aspectRatio;
            OS.WindowSize = new Vector2(newWidth, currentWindowSize.y - 50f);
            OS.WindowPosition = new Vector2((screenSize.x / 2) - newWidth / 2, 0);
        }

    }
}