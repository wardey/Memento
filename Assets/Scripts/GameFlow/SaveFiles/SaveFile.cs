using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveFile
{
    //the data for a savefile should all be stored in an instance of this class to be saved
    //Note: adding new things to the savefile requires it to be updated in storyProgress class and gamecontroller class such that it is correctly updated
    //upon save or load, in addition to its implementation.
    //namely, storyProgress and game controller both needs a copy of whatever's added, and add code accordingly in 
    //storyprogress's save and load methods
    //gamecontroller's update and init methods 
    //needs a vn mode class, investigation mode class, courtroom mode class, 
    public enum Mode {VN,CourtRoom,Investigation,FreeTime, LogicFrenzy, PeaceMaker }
    public static SaveFile current;
    //VN mode class
    public VNModeData mainVNData;   //main story script
    //public VNModeData sideVNData;   not needed in save file//side scripts/dialogues that have to return to the main story when it finishes
    //CourtRoom mode class
    public CourtRoomModeData courtRoomData;
    //investigation mode class
    public InvestigationModeData investigationData;
    //free time mode class
    public FreeTimeModeData freeTimeData;
    //mode flag
    public Mode currMode;

    //static data not belonging to either mode
    public List<string> flagKeys = new List<string>();
    public List<int> flagValues = new List<int>();
    //clues bag
    //public List<Evidence> evidenceBag = new List<Evidence>();

    public SaveFile()
    {

    }
}
