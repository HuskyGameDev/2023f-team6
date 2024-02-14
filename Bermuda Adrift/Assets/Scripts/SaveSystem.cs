using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/*
 *  This program is in charge of both the PlayerData
 *  and MenuData programs. This program creates an
 *  object of either player or menu data, and transfers
 *  its content into a binary file, which is saved on the
 *  users computer. It also translates the binary file
 *  back into an object of either player or menu data 
 *  on request.
 */
public static class SaveSystem {
    
    public static void savePlayer(EnemyManager enemies, Centerpiece center, GameManager game, BuildManager build){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(enemies, center, game, build);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static void saveMenu(){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/menu.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        MenuData data = new MenuData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData loadPlayer(){
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Open);
        if(File.Exists(path) && stream.Length > 0){
            BinaryFormatter formatter = new BinaryFormatter();
            

            PlayerData data = formatter.Deserialize(stream) as PlayerData;



            stream.Close();
            return data;
        }else{
            Debug.LogError("Save file not found in " + path);
            stream.Close();
            return null;
        }
    }

    public static MenuData loadMenu()
    {
        string path = Application.persistentDataPath + "/menu.data";
        FileStream stream = new FileStream(path, FileMode.Open);
        if (File.Exists(path) && stream.Length > 0)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            MenuData data = formatter.Deserialize(stream) as MenuData;

            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            stream.Close();
            return null;
        }
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
