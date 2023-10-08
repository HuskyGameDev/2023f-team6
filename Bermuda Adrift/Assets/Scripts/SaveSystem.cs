using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    
    public static void savePlayer(EnemyManager enemies, Centerpiece center){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(enemies, center);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static PlayerData loadPlayer(){
        string path = Application.persistentDataPath + "/save.data";
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();

            return data;
        }else{
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
