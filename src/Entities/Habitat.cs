using Godot;
using System;

namespace CrazyEaters.Entities
{
    public class Habitat : Spatial
    {
        public Action onReady;

        public override void _Ready()
        {
            if (onReady != null) onReady.Invoke();
        }

    }
}
