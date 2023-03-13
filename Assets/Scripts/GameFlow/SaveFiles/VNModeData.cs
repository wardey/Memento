using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class VNModeData {
    //vn mode data
    public string File;
    public int lineNumber;
    public int lastChoice;  //the line number of the last choice keyword that was read
    public int presentprompt;
    public List<string> CharList;   //in order, left, middle, right (0, 1, 2)
    public string Speaker;
    public string BGM;
    public string BG;
    public string FG;
    public string MC;   //current main character, only used for changing speaker without sprite on screen
}
