using Godot;
using System;

namespace CrazyEaters.Controllers.Launcher
{

    public interface IThrowable
    {
        void Drop(Vector3 force);
        void Hold(Spatial slot);
    }
}