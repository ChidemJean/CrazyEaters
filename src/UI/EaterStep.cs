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
                var thumb = characterAgeThumbPrefab.Instance<TextureRect>();
                thumb.Texture = age.icon;
                characterAgeThumbs.AddChild(thumb);
            }
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
