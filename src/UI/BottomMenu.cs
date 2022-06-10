namespace CrazyEaters.UI
{
    using Godot;
    using System;
    using CrazyEaters.Resources;
    using CrazyEaters.Managers;
    using CrazyEaters.Controllers;

    public class BottomMenu : PanelContainer
    {
        [Export(hintString: "Slider Trigger")]
        public NodePath slideTriggerPath;
        
        [Export]
        public bool open = false;

        [Export]
        public NodePath tweenPath;
        [Export]
        public NodePath blockItemsContainerPath;

        [Export]
        public PackedScene blockItemPrefab;

        [Export]
        public BlocksData blocksData;

        private Control slideTrigger;
        private GridContainer blockItemsContainer;

        private Tween tween;

        private Vector2 initialRectPosition;

        private BlockItem selectedItem;
        private SceneSwitcher sceneSwitcher;
        private PlacementController placementController;

        public override void _Ready()
        {
            sceneSwitcher = GetNode<SceneSwitcher>("/root/MainNode/SceneSwitcher");
            initialRectPosition = GetGlobalRect().Position;
            slideTrigger = GetNode<Control>(slideTriggerPath);
            blockItemsContainer = GetNode<GridContainer>(blockItemsContainerPath);
            tween = GetNode<Tween>(tweenPath);
            //TODO: esquema de injeacao de dependencia, ou coloar no global
            placementController = ((HabitatScene) sceneSwitcher.currentScene).placementController;
            CreateBlockItems();
            StartAnimation();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                InputEventMouseButton _event = (InputEventMouseButton) @event;

                if (_event.IsPressed() && _event.ButtonIndex == (int) ButtonList.Left) {
                    bool mouseInside = slideTrigger.GetGlobalRect().HasPoint(_event.Position);
                    if (mouseInside) {
                        Toggle();
                    }
                }
            }
        }

        public void Toggle()
        {
            this.open = !this.open;
            StartAnimation();
        }

        public void StartAnimation() 
        {
            if (this.open) OpenAnimation();
            if (!this.open) CloseAnimation();
        }

        public async void CloseAnimation()
        {
            tween.StopAll();
            tween.InterpolateProperty(this, "rect_position:y", GetGlobalRect().Position.y, GetViewport().Size.y - slideTrigger.GetGlobalRect().Size.y, .6f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            tween.Start();
            await ToSignal(tween, "tween_completed");
        }

        public async void OpenAnimation()
        {
            tween.StopAll();
            tween.InterpolateProperty(this, "rect_position:y", GetGlobalRect().Position.y, initialRectPosition.y, .8f, Tween.TransitionType.Sine, Tween.EaseType.Out);
            tween.Start();
            await ToSignal(tween, "tween_completed");
        }

        public void CreateBlockItems()
        {
            foreach (string key in blocksData.blocks.Keys) {
                BlockData blockData = blocksData.blocks[key];
                BlockItem item = blockItemPrefab.Instance<BlockItem>();
                blockItemsContainer.AddChild(item);
                item.SetBlockData(blockData);
                item.bottomMenu = this;
            }
        }

        public void ChangeBlock(BlockItem item)
        {
            if (selectedItem != null) selectedItem.SetSelect(false);
            selectedItem = item;
            selectedItem.SetSelect(true);
            placementController.currentBlockId = selectedItem.blockData.id.ToInt();
        }

    }
}