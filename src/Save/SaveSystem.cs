
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
                --- id quest e data de conclusão;
                --- ids entities unblocked;
                --- yellow coins;
                --- jelly gems;
            SAVE DE INVENTARIO:
                --- ids de entities e suas qtds;
            SAVE DE MULTIPLAYER:
                --- friends nick;
                --- convites;
        */
        public static Task SaveGame(GameData gameData)
        {
            return Task.Run(() => { 
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                string path = Godot.OS.GetUserDataDir() + "/game.save";
                FileStream fileStream = new FileStream(path, FileMode.Create);

                binaryFormatter.Serialize(fileStream, gameData);
                fileStream.Close();
            });
        }

        public static Task<GameData> LoadGame()
        {
            return Task.Run(() => { 
                string path = Godot.OS.GetUserDataDir() + "/game.save";
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
