using Godot;
using System;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Save;

namespace CrazyEaters.Managers
{
    public class AccountManager : Node
    {
        [Inject] private GameManager gameManager;
        [Inject] private SaveSystemNode saveSystemNode;
        [Export] private string nickname = "Default Nick 126";
        [Export] private int level = 1;
        [Export] private int xp = 0;
        [Export] private int xpForNextLevel = 1000;
        [Export] private int coins = 0;
        [Export] private int beans = 0;

        public string Nickname { get { return nickname; } }
        public int Level { 
            get { return level; }
            set {
                this.level = value;
                SaveLevel();
            }
        }
        public int Xp { 
            get { return xp; }
            set {
                this.xp = value;
                SaveXp();
            }
        }
        public int XpForNextLevel { get { return xpForNextLevel; } }
        public int Coins { 
            get { return coins; }
            set {
                this.coins = value;
                SaveCoins();
            }
        }
        public int Beans { 
            get { return beans; }
            set {
                this.beans = value;
                SaveBeans();
            }
        }

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

        public void Save()
        {
            saveSystemNode.LoadAccount((data) => {
                UserData userData = data.userData;
                userData.level = Level;
                userData.xp = Xp;
                userData.yellowCoins = Coins;
                userData.jellyGems = Beans;
                data.userData = userData;
                saveSystemNode.SaveAccount(data);
            });
        }

        public void SaveLevel()
        {
            gameManager.TriggerEvent(GameEvent.LevelUpdate, level);
            saveSystemNode.LoadAccount((data) => {
                UserData userData = data.userData;
                userData.level = Level;
                data.userData = userData;
                saveSystemNode.SaveAccount(data);
            });
        }

        public void SaveXp()
        {
            gameManager.TriggerEvent(GameEvent.XpUpdate, xp);
            saveSystemNode.LoadAccount((data) => {
                UserData userData = data.userData;
                userData.xp = Xp;
                data.userData = userData;
                saveSystemNode.SaveAccount(data);
            });
        }

        public void SaveCoins()
        {
            gameManager.TriggerEvent(GameEvent.CoinsUpdate, coins);
            saveSystemNode.LoadAccount((data) => {
                UserData userData = data.userData;
                userData.yellowCoins = Coins;
                data.userData = userData;
                saveSystemNode.SaveAccount(data);
            });
        }

        public void SaveBeans()
        {
            gameManager.TriggerEvent(GameEvent.BeansUpdate, beans);
            saveSystemNode.LoadAccount((data) => {
                UserData userData = data.userData;
                userData.jellyGems = Beans;
                data.userData = userData;
                saveSystemNode.SaveAccount(data);
            });
        }
    }
}