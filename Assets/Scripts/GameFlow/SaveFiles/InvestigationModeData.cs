using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InvestigationModeData {

    public int lineNumber;
    public List<Location> scenes = new List<Location>();   //locations
    public List<string> completeScenes; //locations that must be completed before round is complete
    public string bgm;
    public string noleaveFile;
    public string currLocation;
    public string startLocation;
    public string startScript;
    public string allCompleteFile;
    public string shorcutFile = "";
    public string nextFile, nextMode;
}
