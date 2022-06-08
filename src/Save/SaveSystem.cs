
namespace CrazyEaters.Save
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class SaveSystem
    {
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
