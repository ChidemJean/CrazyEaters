using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Save;

namespace CrazyEaters.Managers
{
    public class AccountManager : Node
    {
        [Inject] private SaveSystemNode saveSystemNode;
        [Export] private string nickname = "Default Nick 126";
        [Export] private int level = 1;
        [Export] private int xp = 0;
        [Export] private int xpForNextLevel = 1000;
        [Export] private int coins = 0;
        [Export] private int beans = 0;

        public string Nickname { get { return nickname; } }
        public int Level { get { return level; } }
        public int Xp { get { return xp; } }
        public int XpForNextLevel { get { return xpForNextLevel; } }
        public int Coins { get { return coins; } }
        public int Beans { get { return beans; } }

        public override void _Ready()
        {
            this.ResolveDependencies();
            saveSystemNode.LoadAccount(OnLoadAccount);
        }

        public void OnLoadAccount(AccountData accountData)
        {
            if (accountData == null) return;
            this.nickname = accountData.userData.nickname;
            this.level = accountData.userData.level;
            this.xp = accountData.userData.xp;
        }
    }
}
