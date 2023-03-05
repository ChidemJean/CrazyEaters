using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class HabitatStep : GameCreateStep
    {
        [Inject] private ItemsManager itemsManager;
        [Export] private NodePath railSliderPath;
        [Export] private NodePath sliderPath;
        [Export] private PackedScene habitatSliderImgPrefab;
        [Export] private NodePath namePath;
        [Export] private NodePath inputNamePath;
        [Export] private NodePath itemStatsPath;
        [Export] private PackedScene itemStatPrefab;
        [Export] private PackedScene gridStatsPrefab;

        private Control itemStats;
        private Label name;
        private LineEdit inputName;
        private Control railSlider;
        private SliderSelector slider;
        SceneTreeTween tween;
        private List<CrazyEaters.Resources.HabitatData> habitats;

        public override void _Ready()
        {
            this.ResolveDependencies();
            itemStats = GetNode<Control>(itemStatsPath);
            name = GetNode<Label>(namePath);
            inputName = GetNode<LineEdit>(inputNamePath);
            slider = GetNode<SliderSelector>(sliderPath);
            railSlider = GetNode<Control>(railSliderPath);

            habitats = itemsManager.GetHabitats(true);

            CreateSliders();
        }

        public void CreateSliders()
        {
            foreach (var habitat in habitats) {
                var newSlide = habitatSliderImgPrefab.Instance<SliderSelectorItem>();
                newSlide.Texture = habitat.thumb;
                newSlide.Key = habitat.id;
                railSlider.AddChild(newSlide);
            }
            slider.PopulateTabs();
            slider.Connect("slideChange", this, nameof(OnSlideChange));
        }

        public CrazyEaters.Resources.HabitatData GetHabitatDataByKey(string key)
        {
            foreach (var habitat in habitats) {
                if (habitat.id == key) {
                    return habitat;
                }
            }
            return null;
        }

        public void OnSlideChange(string key)
        {
            var habitat = GetHabitatDataByKey(key);
            if (habitat == null) return;
            name.Text = habitat.name;
            inputName.Text = habitat.name;
            keySelected = habitat.id;
            UpdateStats(habitat);
        }

        public void UpdateStats(CrazyEaters.Resources.HabitatData data)
        {
            foreach (Node child in itemStats.GetChildren()){
                itemStats.RemoveChild(child);
                child.QueueFree();
            }
            GridContainer gridContainer = gridStatsPrefab.Instance<GridContainer>();
            gridContainer.Name = $"Grid";
            gridContainer.Visible = false;
            itemStats.AddChild(gridContainer);

            // ItemStat item = itemStatPrefab.Instance<ItemStat>();
            // gridContainer.AddChild(item);
            // item.Name = stat.Value.name;
            // item.Value = stat.Value.max.ToString();
            // item.Icon = stat.Value.icon;
            // if (stat.Value.variation != 0) {
            //     item.Bonus = $"{stat.Value.variation} / {stat.Value.seconds}s";
            // }
        }

        public override void Hide(int dir, bool animate)
        {
            base.Hide(dir, animate);
        }
        public override async void Show(int dir, bool animate)
        {
            base.Show(dir, animate);
            await Task.Delay(TimeSpan.FromMilliseconds(950));
            slider.AnimateToPanel();
        }
    }
}
