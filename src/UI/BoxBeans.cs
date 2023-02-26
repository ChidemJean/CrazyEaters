using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class BoxBeans : BoxMoney
    {
        public override void Setup()
        {
            label.Text = accountManager.Beans.ToString();
        }
    }
}