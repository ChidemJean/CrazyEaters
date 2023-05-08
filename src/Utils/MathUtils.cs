using Godot;
using System;

namespace CrazyEaters.Utils
{
    public class MathUtils
    {
        public static int RandiRange(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max);
        }
    }
}