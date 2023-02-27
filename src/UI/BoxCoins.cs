using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class BoxCoins : BoxMoney
    {

        public override void _Ready()
        {
            base._Ready();
            gameManager.StartListening(GameEvent.CoinsUpdate, OnCoinsUpdate);
        }

        public override void Setup()
        {
            label.Text = accountManager.Coins.ToString();
        }

        public void OnCoinsUpdate(object _coins)
        {
            int coins = (int) _coins;
            label.Text = coins.ToString();
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.CoinsUpdate, OnCoinsUpdate);
        }
    }
}