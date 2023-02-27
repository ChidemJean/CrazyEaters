using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class UserInfs : Control
    {
        [Inject] private GameManager gameManager;
        [Inject] private AccountManager accountManager;
        [Export] private NodePath nicknameLabelPath;
        [Export] private NodePath levelLabelPath;
        [Export] private NodePath xpLabelPath;
        [Export] private NodePath xpNextLevelLabelPath;
        [Export] private NodePath xpBarPath;

        private Label nicknameLabel;
        private Label levelLabel;
        private Label xpLabel;
        private Label xpNextLevelLabel;
        private NinePatchRect xpBar;
        private SceneTreeTween tween;

        public override void _Ready()
        {
            this.ResolveDependencies();

            nicknameLabel = GetNode<Label>(nicknameLabelPath);
            levelLabel = GetNode<Label>(levelLabelPath);
            xpLabel = GetNode<Label>(xpLabelPath);
            xpNextLevelLabel = GetNode<Label>(xpNextLevelLabelPath);
            xpBar = GetNode<NinePatchRect>(xpBarPath);
            gameManager.StartListening(GameEvent.LevelUpdate, OnLevelUpdate);
            gameManager.StartListening(GameEvent.XpUpdate, OnXpUpdate);

            SetupData();
        }

        public void SetupData()
        {
            nicknameLabel.Text = accountManager.Nickname;
            levelLabel.Text = accountManager.Level.ToString();
            xpLabel.Text = accountManager.Xp.ToString();
            xpNextLevelLabel.Text = accountManager.XpForNextLevel.ToString();
            UpdateXpBarUI();
        }

        public void UpdateXpBarUI()
        {
            float totalW = xpBar.GetParent<Control>().RectSize.x;
            float ptg = (float)accountManager.Xp / (float)accountManager.XpForNextLevel;

            if (tween != null) {
                tween.Kill();
            }
            tween = GetTree().CreateTween();
            tween.TweenProperty(xpBar, "rect_size:x", totalW * ptg, 1f);
        }

        public void OnLevelUpdate(object _level) 
        {
            int level = (int) _level;
            levelLabel.Text = level.ToString();
        }

        public void OnXpUpdate(object _xp) 
        {
            int xp = (int) _xp;
            xpLabel.Text = xp.ToString();
            UpdateXpBarUI();
        }

        public override void _ExitTree()
        {
            gameManager.StopListening(GameEvent.LevelUpdate, OnLevelUpdate);
            gameManager.StopListening(GameEvent.XpUpdate, OnXpUpdate);
        }
    }
}