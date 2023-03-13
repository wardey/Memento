using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class storyProgress : MonoBehaviour
{
    //make this class static
    //this class keeps track of game data at run-time, mediator between savefiles and gamecontroller

    public Action onLoadGame;
    public Action onNewGame;
    public Action onSaveGame;

    public static storyProgress instance;

    public string newGameStartFile;
    public SaveFile.Mode startMode;

    public VNModeData mainVNData;
    public CourtRoomModeData courtRoomData;
    public InvestigationModeData investigationData;
    public FreeTimeModeData freeTimeData;
    public SaveFile.Mode currMode;

    //public List<Evidence> evidenceBag = new List<Evidence>();

    public int currSaveFile;

    //some way to save the flags throughout the game
    public Dictionary<string, int> flags = new Dictionary<string, int>();
    public Dictionary<string, string> mostRecentSprite = new Dictionary<string, string>();

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }

    public void Save(int index) //index starts at 1
    {
        //tells game controller to send newest data
        onSaveGame();
        //actually saving current savefile after getting most recent info
        SaveFile temp = new SaveFile();
        temp.mainVNData = mainVNData;
        temp.courtRoomData = courtRoomData;
        temp.investigationData = investigationData;
        temp.freeTimeData = freeTimeData;
        temp.currMode = currMode;
        //get evidence bag
        //update flags too
        foreach (KeyValuePair<string, int> pair in flags)
        {
            temp.flagKeys.Add(pair.Key);
            temp.flagValues.Add(pair.Value);
        }
        //update current savefile with most recent data
        SaveFile.current = temp;
        LoadSave.Save(index);
    }

    public void autoSave()
    {
        //new game, hasnt picked a savefile yet, gonna disable function for that
        if (currSaveFile == 0)
        {
            return;
        }
        else
        {
            //maybe play a little autosave animation
            Save(currSaveFile);
        }
    }

    public void Load(int index) //index starts at 1
    {
        LoadSave.Load(index);
        mainVNData = SaveFile.current.mainVNData;
        courtRoomData = SaveFile.current.courtRoomData;
        investigationData = SaveFile.current.investigationData;
        freeTimeData = SaveFile.current.freeTimeData;
        currMode = SaveFile.current.currMode;
        //evidenceBag = SaveFile.current.evidenceBag;
        //get flags from savefile too
        List<string> keys = SaveFile.current.flagKeys;
        List<int> values = SaveFile.current.flagValues;
        int length = keys.Count;
        flags.Clear();
        for (int i = 0; i < length; i++)
        {
            flags[keys[i]] = values[i];
        }
        currSaveFile = index;
        onLoadGame();
    }

    public void newGame()
    {
        mainVNData = new VNModeData();
        mainVNData.File = newGameStartFile;
        mainVNData.lineNumber = 1;
        currSaveFile = 0;
        onNewGame();
    }
}
