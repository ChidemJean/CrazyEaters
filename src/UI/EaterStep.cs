using Godot;
using System;
using System.Collections.Generic;
using CrazyEaters.Save;
using CrazyEaters.Managers;
using CrazyEaters.Resources;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.UI
{
    public class EaterStep : GameCreateStep
    {
        [Inject] private ItemsManager itemsManager;
        [Export] private NodePath headerPath;
        [Export] private NodePath railSliderPath;
        [Export] private NodePath sliderPath;
        [Export] private PackedScene characterSliderImgPrefab;
        [Export] private NodePath rarityPath;
        [Export] private NodePath namePath;
        [Export] private NodePath inputNamePath;
        [Export] private NodePath characterAgeThumbsPath;
        [Export] private PackedScene characterAgeThumbPrefab;
        [Export] private NodePath itemStatsPath;
        [Export] private PackedScene itemStatPrefab;
        [Export] private PackedScene gridStatsPrefab;

        private Control itemStats;
        private Control characterAgeThumbs;
        private Label rarity;
        private Label name;
        private LineEdit inputName;
        private Control header;
        private Control railSlider;
        private SliderSelector slider;
        SceneTreeTween tween;
        private List<CrazyEaters.Resources.CharacterData> characters;

        public override void _Ready()
        {
            this.ResolveDependencies();
            characterAgeThumbs = GetNode<Control>(characterAgeThumbsPath);
            itemStats = GetNode<Control>(itemStatsPath);
            name = GetNode<Label>(namePath);
            inputName = GetNode<LineEdit>(inputNamePath);
            rarity = GetNode<Label>(rarityPath);
            slider = GetNode<SliderSelector>(sliderPath);
            railSlider = GetNode<Control>(railSliderPath);
            header = GetNode<Control>(headerPath);
            header.RectPosition = new Vector2(0, -header.GetNode<Control>("Main").RectSize.y);

            characters = itemsManager.GetCharacters(true);

            CreateSliders();

            Show();
        }

        public void CreateSliders()
        {
            foreach (var character in characters) {
                var newSlide = characterSliderImgPrefab.Instance<SliderSelectorItem>();
                newSlide.Texture = character.thumb;
                newSlide.Key = character.id;
                railSlider.AddChild(newSlide);
            }
            slider.PopulateTabs();
            slider.Connect("slideChange", this, nameof(OnSlideChange));
        }

        public CrazyEaters.Resources.CharacterData GetCharacterDataByKey(string key)
        {
            foreach (var character in characters) {
                if (character.id == key) {
                    return character;
                }
            }
            return null;
        }

        public void OnSlideChange(string key)
        {
            var character = GetCharacterDataByKey(key);
            if (character == null)  return;
            name.Text = character.name;
            inputName.Text = character.name;
            CustomRarity rarityData = itemsManager.GetCustomRarity(character.rarity);
            if (rarityData != null) {
                rarity.Text = rarityData.name;
                if (rarityData.material == null) {
                    rarity.Set("custom_colors/font_color", rarityData.color);
                    rarity.Material = null;
                } else {
                    rarity.Material = rarityData.material;
                    rarity.Set("custom_colors/font_color", new Color(1,1,1,1));
                }
            }
            UpdateAges(character);
        }

        public void UpdateAges(CrazyEaters.Resources.CharacterData data)
        {
            foreach (Node child in characterAgeThumbs.GetChildren()){
                characterAgeThumbs.RemoveChild(child);
                child.QueueFree();
            }
            foreach (var age in data.agesData) {
                var thumb = characterAgeThumbPrefab.Instance<CharacterAgeThumb>();
                thumb.Texture = age.icon;
                thumb.key = age.name;
                characterAgeThumbs.AddChild(thumb);
            }
            UpdateAgeStats(data);
            SetAgesEvents();
        }

        public void UpdateAgeStats(CrazyEaters.Resources.CharacterData data)
        {
            foreach (Node child in itemStats.GetChildren()){
                itemStats.RemoveChild(child);
                child.QueueFree();
            }
            List<CharacterAgeData> ages = data.agesData;
            foreach (var age in ages) {
                GridContainer gridContainer = gridStatsPrefab.Instance<GridContainer>();
                gridContainer.Name = $"Grid-{age.name}";
                gridContainer.Visible = false;
                itemStats.AddChild(gridContainer);
                StatusesCharacter statuses = age.statusesCharacter;
                foreach (KeyValuePair<string, CrazyEaters.Resources.StatusCharacter> stat in statuses.statuses) {
                    ItemStat item = itemStatPrefab.Instance<ItemStat>();
                    gridContainer.AddChild(item);
                    item.Name = stat.Value.name;
                    item.Value = stat.Value.max.ToString();
                    item.Icon = stat.Value.icon;
                }
            }
            CharacterAgeData initialAge = ages[0];
            UpdateAgeStatsVisible(initialAge.name);
        }

        public void UpdateAgeStatsVisible(string keyAge)
        {
            foreach (Node grid in itemStats.GetChildren()) {
                ((Control) grid).Visible = grid.Name == $"Grid-{keyAge}";
            }
            foreach (Node child in characterAgeThumbs.GetChildren()) {
                CharacterAgeThumb ageThumb = (CharacterAgeThumb) child;
                if (ageThumb.key != keyAge) {
                    ageThumb.ResetModulate();
                } else {
                    ageThumb.Select();
                }
            }
        }

        public void SetAgesEvents() 
        {
            foreach (Node child in characterAgeThumbs.GetChildren()) {
                child.Connect("click", this, nameof(OnAgeThumbClick));
            }
        }

        public void OnAgeThumbClick(string key)
        {
            UpdateAgeStatsVisible(key);
        }

        public void Show()
        {
            if (tween != null) {
                tween.Kill();
            }
            tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            tween.TweenProperty(header, "rect_position:y", 0f, .95f);
        }

        public override async void Hide(int dir, bool animate)
        {
            base.Hide(dir, animate);
        }
        public override async void Show(int dir, bool animate)
        {
            base.Show(dir, animate);
        }
    }
}
