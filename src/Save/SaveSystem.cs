
namespace CrazyEaters.Save
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class SaveSystem
    {
        /*  TODO:
            SAVE PARA CONFIGURAÇÂO DOs CENARIOs: 
                --- id do character, nome, idade, acessorios e seus status;
                --- id dos blocos e suas posicoes;
                --- id do launcher;
            SAVE DE CONTA:
                --- nickname;
                --- level;
                --- xp;
                --- yellow coins;
                --- jelly gems;
                --- id quest e data de conclusão;
                --- ids entities unblocked;
            SAVE DE INVENTARIO:
                --- ids de entities e suas qtds;
            SAVE DE MULTIPLAYER:
                --- friends nick;
                --- convites;
        */

        const string HABITATS_SAVE_KEY = "habitats";
        const string ACCOUNT_SAVE_KEY = "account";
        const string INVENTORY_SAVE_KEY = "inventory";
        const string MULTIPLAYER_SAVE_KEY = "multiplayer";

        public static async Task<bool> SaveHabitatsData(HabitatsGameData habitatsData)
        {
            return await SaveSystem.SaveGame(habitatsData, HABITATS_SAVE_KEY);
        }

        public static async Task<bool> SaveUserData(AccountData accountData)
        {
            return await SaveSystem.SaveGame(accountData, ACCOUNT_SAVE_KEY);
        }

        public static async Task<bool> SaveInventoryData(InventoryData inventoryData)
        {
            return await SaveSystem.SaveGame(inventoryData, INVENTORY_SAVE_KEY);
        }

        public static async Task<bool> SaveMultiplayerData(MultiplayerData multiplayerData)
        {
            return await SaveSystem.SaveGame(multiplayerData, MULTIPLAYER_SAVE_KEY);
        }

        public static async Task<HabitatsGameData> LoadHabitatsData()
        {
            GameData data = await SaveSystem.LoadGame(HABITATS_SAVE_KEY);
            return (HabitatsGameData) data;
        }

        public static async Task<AccountData> LoadUserData()
        {
            GameData data = await SaveSystem.LoadGame(ACCOUNT_SAVE_KEY);
            return (AccountData) data;
        }

        public static async Task<InventoryData> LoadInventoryData()
        {
            GameData data = await SaveSystem.LoadGame(INVENTORY_SAVE_KEY);
            return (InventoryData) data;
        }

        public static async Task<MultiplayerData> LoadMultiplayerData()
        {
            GameData data = await SaveSystem.LoadGame(MULTIPLAYER_SAVE_KEY);
            return (MultiplayerData) data;
        }

        private static Task<bool> SaveGame(GameData gameData, string identifier)
        {
            return Task.Run<bool>(() => { 
                try {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    string path = Godot.OS.GetUserDataDir() + $"/{identifier}.save";
                    FileStream fileStream = new FileStream(path, FileMode.Create);

                    binaryFormatter.Serialize(fileStream, gameData);
                    fileStream.Close();
                } catch (System.Exception e) {
                    return false;
                }
                return true;
            });
        }

        private static Task<GameData> LoadGame(string identifier)
        {
            return Task.Run(() => { 
                string path = Godot.OS.GetUserDataDir() + $"/{identifier}.save";
                if (File.Exists(path)) {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    FileStream fileStream = new FileStream(path, FileMode.Open);
                    GameData data = (GameData) binaryFormatter.Deserialize(fileStream);
                    fileStream.Close();
                    return data;
                } else {
                    return null;
                }
            });
        }
    }
}
