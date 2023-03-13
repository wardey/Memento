using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class LoadSave {

    public static List<SaveFile> savedGames = new List<SaveFile>();

    public static void Save(int index)      //overwrite a savefile if it already exists, or add another one to the list if slot isnt used, index starts at 1
    {
        if (index > LoadSave.savedGames.Count)
            LoadSave.savedGames.Add(SaveFile.current);
        else
            LoadSave.savedGames[index-1] = SaveFile.current;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, LoadSave.savedGames);
        file.Close();
    }

    public static void Load()   //for loading all savefile at beginning of game
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            LoadSave.savedGames = (List<SaveFile>)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void Load(int index)      //load a savefile at a specific slot/index, slots with no savefile shouldn't be allowed to be clicked
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            LoadSave.savedGames = (List<SaveFile>)bf.Deserialize(file);
            file.Close();
        }
        SaveFile.current = LoadSave.savedGames[index-1];
    }
}
