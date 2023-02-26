using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class BoxCoins : BoxMoney
    {
        public override void Setup()
        {
            label.Text = accountManager.Coins.ToString();
        }
    }
}