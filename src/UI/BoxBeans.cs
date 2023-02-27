using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class BoxBeans : BoxMoney
    {
        public override void _Ready()
        {
            base._Ready();
            gameManager.StartListening(GameEvent.BeansUpdate, OnBeansUpdate);
        }
        public override void Setup()
        {
            label.Text = accountManager.Beans.ToString();
        }
        public void OnBeansUpdate(object _beans)
        {
            int beans = (int) _beans;
            label.Text = beans.ToString();
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.BeansUpdate, OnBeansUpdate);
        }
    }
}