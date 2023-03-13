using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FreeTimeModeData
{
    public int lineNumber;
    public List<Location> scenes = new List<Location>();   //locations
    public string bgm;
    public string currLocation;
    public string startLocation;
    public string startScript;
    public string nextFile, nextMode;
}
