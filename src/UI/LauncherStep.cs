using Godot;
using System;
using System.Collections.Generic;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.UI
{
    public class LauncherStep : GameCreateStep
    {
        [Inject] private ItemsManager itemsManager;
        [Export] private NodePath launcherItemsPath;
        [Export] private PackedScene launcherItemPrefab;

        private Control launcherItems;
        private List<CrazyEaters.Resources.LauncherData> launchers;

        public override void _Ready()
        {
            this.ResolveDependencies();
            launcherItems = GetNode<Control>(launcherItemsPath);
            launchers = itemsManager.GetLaunchers();
            PopulateItems();
        }

        public void PopulateItems()
        {
            foreach (var launcher in launchers) {
                var newItem = launcherItemPrefab.Instance<LauncherItem>();
                newItem.Key = launcher.id;
                newItem.onReady = () => {
                    newItem.Thumb = launcher.thumb;
                    newItem.Rarity = launcher.rarity;
                    newItem.IsLocked(launcher._blocked || !launcher.unblockedByDefault);
                };
                newItem.Connect("click", this, nameof(OnItemClick));
                launcherItems.AddChild(newItem);
            }
        }

        public void OnItemClick(string key)
        {
            keySelected = key;
            foreach (var launcherItem in launcherItems.GetChildren()) {
                LauncherItem item = (LauncherItem) launcherItem;
                item.IsSelected = item.Key == key;
            }
        }

        public override void Hide(int dir, bool animate)
        {
            base.Hide(dir, animate);
        }
        public override void Show(int dir, bool animate)
        {
            base.Show(dir, animate);
        }
    }
}
