using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    
    public static void savePlayer(EnemyManager enemies, Centerpiece center, GameManager game, BuildManager build){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(enemies, center, game, build);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static PlayerData loadPlayer(){
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Open);
        if(File.Exists(path) && stream.Length > 0){
            BinaryFormatter formatter = new BinaryFormatter();
            

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            

            return data;
        }else{
            Debug.LogError("Save file not found in " + path);
            return null;
        }
        stream.Close();
    }

    public static void resetPlayer(){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void deletePlayer(){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData();

        formatter.Serialize(stream, data);

        stream.Close();
        File.Delete(path);

    }
}
