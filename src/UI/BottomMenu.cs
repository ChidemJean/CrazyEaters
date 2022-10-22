namespace CrazyEaters.UI
{
    using Godot;
    using System;
    using CrazyEaters.Resources;
    using CrazyEaters.Managers;
    using CrazyEaters.Controllers;
    using CrazyEaters.DependencyInjection;

    public class BottomMenu : Control
    {
        [Export(hintString: "Slider Trigger")]
        public NodePath slideTriggerPath;

        [Export]
        public NodePath slideTriggerProgressPath;
        
        [Export]
        public NodePath offsetControlPath;
        
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

        [Export]
        public float timeToOpenModeBtns = 3f;

        private Control offsetControl;
        private Control slideTrigger;
        private TextureProgress slideTriggerProgress;
        private GridContainer blockItemsContainer;

        private Tween tween;

        private Vector2 initialRectPosition;

        private BlockItem selectedItem;
        private SceneSwitcher sceneSwitcher;
        public PlacementController placementController;
        
        private bool slideTriggerClicked = false;
        private bool modeBtnsOpen = false;
        private Vector2 initialClickPosition;
        private Vector2 currentMousePosition;
        private Vector2 currentMouseRelative;
        private float timeSliderTriggerHolded = 0f;
        private float menuBottomInitialY = 0f;

        [Inject]
        public GameManager gameManager;

        public override void _Ready()
        {
            this.ResolveDependencies();
            sceneSwitcher = GetNode<SceneSwitcher>("/root/MainNode/SceneSwitcher");
            initialRectPosition = GetGlobalRect().Position;
            offsetControl = GetNode<Control>(offsetControlPath);
            slideTrigger = GetNode<Control>(slideTriggerPath);
            slideTriggerProgress = GetNode<TextureProgress>(slideTriggerProgressPath);
            blockItemsContainer = GetNode<GridContainer>(blockItemsContainerPath);
            menuBottomInitialY = RectGlobalPosition.y;
            tween = GetNode<Tween>(tweenPath);
            CreateBlockItems();
            StartAnimation();
            // RectMinSize = new Vector2(0, GetViewport().Size.y);
            // GetViewport().Connect("size_changed", this, nameof(OnViewportSizeChanged));
        }

        public void OnViewportSizeChanged()
        {
            RectMinSize = new Vector2(0, GetViewport().Size.y);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                InputEventMouseButton _event = (InputEventMouseButton) @event;
                bool leftMouse = _event.ButtonIndex == (int) ButtonList.Left;

                if (_event.IsPressed() && leftMouse) {
                    if (PointInsideSlideTrigger(_event.Position)) {
                        // Toggle();
                        slideTriggerClicked = true;
                        initialClickPosition = currentMousePosition = _event.Position;
                        gameManager.inputMode = GameManager.InputMode.UI;
                    }
                    return;
                }

                if (!_event.IsPressed() && leftMouse) {
                    if (slideTriggerClicked) {
                        slideTriggerClicked = false;
                        initialClickPosition = Vector2.Zero;
                        timeSliderTriggerHolded = 0;
                        slideTriggerProgress.Value = 0;
                        if (modeBtnsOpen) {
                            HideModeBtns();
                        }
                        if (gameManager.inputMode == GameManager.InputMode.UI) gameManager.inputMode = GameManager.InputMode.SCENE;
                        if (currentMouseRelative.y > 0) {
                            CloseAnimation();
                        } else {
                            OpenAnimation();
                        }
                        return;
                    }
                }

                return;
            }

            if (@event is InputEventMouseMotion) {
                InputEventMouseMotion _event = (InputEventMouseMotion) @event;
                if (slideTriggerClicked) {
                    currentMousePosition = _event.Position;
                    currentMouseRelative = _event.Relative;
                }
            }
        }

        public override void _Process(float delta) 
        {
            if (slideTriggerClicked) {
                if (PointInsideSlideTrigger((Vector2) currentMousePosition)) {
                    timeSliderTriggerHolded += delta;
                    slideTriggerProgress.Value = (timeSliderTriggerHolded * 100) / timeToOpenModeBtns;
                    if (timeSliderTriggerHolded >= timeToOpenModeBtns || !modeBtnsOpen) {
                        ShowModeBtns();
                    }
                } else {
                    timeSliderTriggerHolded = 0f;
                    ChangeBottomPosition(((Vector2) initialClickPosition).y - ((Vector2) currentMousePosition).y);
                }
            }
        }

        public void ShowModeBtns()
        {
            modeBtnsOpen = true;
        }

        public void HideModeBtns()
        {
            modeBtnsOpen = false;
        }

        public bool PointInsideSlideTrigger(Vector2 pos)
        {
            return slideTrigger.GetGlobalRect().HasPoint(pos);
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

        public void ChangeBottomPosition(float posY)
        {
            RectGlobalPosition = new Vector2(RectGlobalPosition.x, menuBottomInitialY - posY);
        }

        public async void CloseAnimation()
        {
            tween.StopAll();
            tween.InterpolateProperty(this, "rect_position:y", GetGlobalRect().Position.y, GetViewport().Size.y - offsetControl.GetGlobalRect().Size.y, .6f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            tween.Start();
            await ToSignal(tween, "tween_completed");
            menuBottomInitialY = RectGlobalPosition.y;
        }

        public async void OpenAnimation()
        {
            tween.StopAll();
            tween.InterpolateProperty(this, "rect_position:y", GetGlobalRect().Position.y, initialRectPosition.y, .8f, Tween.TransitionType.Sine, Tween.EaseType.Out);
            tween.Start();
            await ToSignal(tween, "tween_completed");
            menuBottomInitialY = RectGlobalPosition.y;
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
            ((HabitatScene)sceneSwitcher.currentScene).currentBlockId = selectedItem.blockData.id.ToInt();
        }

    }
}