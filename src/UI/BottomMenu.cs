namespace CrazyEaters.UI
{
    using Godot;
    using System;
    using System.Collections.Generic;
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
        public NodePath inventoryItemsContainerPath;
        [Export]
        public NodePath recipesItemsContainerPath;

        [Export]
        public PackedScene blockItemPrefab;
        [Export]
        public PackedScene inventoryItemPrefab;

        [Export]
        public float timeToOpenModeBtns = 3f;

        private Control offsetControl;
        private Control slideTrigger;
        private TextureProgress slideTriggerProgress;
        private GridContainer blockItemsContainer;
        private GridContainer inventoryItemsContainer;
        private GridContainer recipesItemsContainer;

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

        private List<BtnMode> btnsMode;
        private SceneTreeTween _tween;

        public override void _Ready()
        {
            this.ResolveDependencies();
            sceneSwitcher = GetNode<SceneSwitcher>("/root/MainNode/SceneSwitcher");
            initialRectPosition = GetGlobalRect().Position;
            offsetControl = GetNode<Control>(offsetControlPath);
            slideTrigger = GetNode<Control>(slideTriggerPath);
            slideTriggerProgress = GetNode<TextureProgress>(slideTriggerProgressPath);
            blockItemsContainer = GetNode<GridContainer>(blockItemsContainerPath);
            inventoryItemsContainer = GetNode<GridContainer>(inventoryItemsContainerPath);
            recipesItemsContainer = GetNode<GridContainer>(recipesItemsContainerPath);
            menuBottomInitialY = RectGlobalPosition.y;
            tween = GetNode<Tween>(tweenPath);
            OnGameModeChange(gameManager.gameMode);
            StartAnimation();

            btnsMode = new List<BtnMode>();
            btnsMode.Add(slideTrigger.GetNode<BtnMode>("BtnModeBuild"));
            btnsMode.Add(slideTrigger.GetNode<BtnMode>("BtnModeLauncher"));
            btnsMode.Add(slideTrigger.GetNode<BtnMode>("BtnModeCook"));
            // RectMinSize = new Vector2(0, GetViewport().Size.y);
            // GetViewport().Connect("size_changed", this, nameof(OnViewportSizeChanged));

            gameManager.StartListening(GameEvent.GameModeChange, OnGameModeChange);
        }

        public void OnGameModeChange(object mode)
        {
            if (mode is GameManager.GameMode)
            {
                blockItemsContainer.Visible = false;
                inventoryItemsContainer.Visible = false;
                recipesItemsContainer.Visible = false;

                switch (mode) {
                    case GameManager.GameMode.BUILD:
                        ShowBlockItems();
                        blockItemsContainer.Visible = true;
                        break;
                    case GameManager.GameMode.LAUNCHER:
                        ShowInventoryItems();
                        inventoryItemsContainer.Visible = true;
                        break;
                    case GameManager.GameMode.COOK:
                        ShowRecipesItems();
                        recipesItemsContainer.Visible = true;
                        break;
                }
            }
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
                        if (gameManager.inputMode == GameManager.InputMode.UI) gameManager.inputMode = GameManager.InputMode.SCENE;
                        if (modeBtnsOpen) {
                            HideModeBtns();
                            return;
                        }
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
                    if (timeSliderTriggerHolded >= timeToOpenModeBtns && !modeBtnsOpen) {
                        ShowModeBtns();
                    }
                } else if (!modeBtnsOpen) {
                    timeSliderTriggerHolded = 0f;
                    ChangeBottomPosition(((Vector2) initialClickPosition).y - ((Vector2) currentMousePosition).y);
                }
            }
        }

        public void ShowModeBtns()
        {
            modeBtnsOpen = true;
            _tween = GetTree().CreateTween();
            Vector2 middleSliderTrigger = Vector2.Zero;// + new Vector2(slideTrigger.GetGlobalRect().Size.x / 2, slideTrigger.GetGlobalRect().Size.y / 2);
            
            btnsMode[0].Modulate = new Color(1, 1, 1, 1);
            btnsMode[1].Modulate = new Color(1, 1, 1, 1);
            btnsMode[2].Modulate = new Color(1, 1, 1, 1);

            _tween.TweenProperty(btnsMode[0], "rect_position", middleSliderTrigger - new Vector2(160, 180), .2f);
            _tween.Parallel().TweenProperty(btnsMode[1], "rect_position", middleSliderTrigger - new Vector2(0, 240), .2f).SetDelay(.05f);
            _tween.Parallel().TweenProperty(btnsMode[2], "rect_position", middleSliderTrigger - new Vector2(-160, 180), .2f).SetDelay(.1f);
            _tween.TweenCallback(this, nameof(OnAnimBtnModeShowEnd));
            _tween.Play();
        }

        public void OnAnimBtnModeShowEnd()
        {
            foreach (BtnMode btnMode in btnsMode) {
                btnMode.IsShowing = true;
            }
        }

        public void HideModeBtns()
        {
            if (_tween != null) _tween.Stop();
            _tween = GetTree().CreateTween();
            modeBtnsOpen = false;
            foreach (BtnMode btnMode in btnsMode) {
                btnMode.IsShowing = false;
            }

            _tween.TweenProperty(btnsMode[0], "rect_position", Vector2.Zero, .2f);
            _tween.Parallel().TweenProperty(btnsMode[0], "modulate:a", 0f, .2f).SetDelay(.025f);
            _tween.Parallel().TweenProperty(btnsMode[1], "rect_position", Vector2.Zero, .2f).SetDelay(.05f);
            _tween.Parallel().TweenProperty(btnsMode[1], "modulate:a", 0f, .2f).SetDelay(.05f);
            _tween.Parallel().TweenProperty(btnsMode[2], "rect_position", Vector2.Zero, .2f).SetDelay(.075f);
            _tween.Parallel().TweenProperty(btnsMode[2], "modulate:a", 0f, .2f).SetDelay(.15f);
            _tween.TweenCallback(this, nameof(OnAnimBtnModeHideEnd));
            _tween.Play();
        }

        public void OnAnimBtnModeHideEnd()
        {
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

        public void ShowBlockItems()
        {
            if (blockItemsContainer.GetChildCount() > 0) {
                return;
            }
            // foreach (string key in blocksData.blocks.Keys) {
            //     BlockData blockData = blocksData.blocks[key];
            //     BlockItem item = blockItemPrefab.Instance<BlockItem>();
            //     blockItemsContainer.AddChild(item);
            //     item.SetBlockData(blockData);
            //     item.bottomMenu = this;
            // }
        }

        public void ShowInventoryItems()
        {
            if (inventoryItemsContainer.GetChildCount() > 0) {
                return;
            }
            // foreach (BuyableEntityData data in buyablesData.buyables) {
            //     InventoryItem inventoryItem = inventoryItemPrefab.Instance<InventoryItem>();
            //     inventoryItemsContainer.AddChild(inventoryItem);
            //     inventoryItem.SetBuyableData(data);
            //     inventoryItem.bottomMenu = this;
            // }
        }

        public void ShowRecipesItems()
        {
            if (recipesItemsContainer.GetChildCount() > 0) {
                return;
            }
            // foreach (BuyableEntityData key in buyablesData.buyables) {
                
            // }
        }

        public void ChangeBlock(BlockItem item)
        {
            if (selectedItem != null) selectedItem.SetSelect(false);
            selectedItem = item;
            selectedItem.SetSelect(true);
            ((HabitatScene)sceneSwitcher.currentScene).currentBlockId = selectedItem.blockData.id.ToInt();
        }

        public override void _ExitTree() {
            gameManager.StopListening(GameEvent.GameModeChange, OnGameModeChange);
        }

    }
}