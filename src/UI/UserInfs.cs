using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.UI
{
    public class UserInfs : Control
    {
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
            GD.Print(ptg);
        }
    }
}