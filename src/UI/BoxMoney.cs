using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public abstract class BoxMoney : Control
    {
        [Inject] protected AccountManager accountManager;
        [Inject] protected GameManager gameManager;
        [Export] protected NodePath labelPath;
        [Export] protected NodePath iconPath;
        [Export] protected NodePath btnPath;

        protected Label label;
        protected TextureRect icon;
        protected Control btn;

        public override void _Ready()
        {
            this.ResolveDependencies();
            this.label = GetNode<Label>(labelPath);
            this.icon = GetNode<TextureRect>(iconPath);
            this.btn = GetNode<Control>(btnPath);
            Setup();
        }

        public abstract void Setup();
    }
}