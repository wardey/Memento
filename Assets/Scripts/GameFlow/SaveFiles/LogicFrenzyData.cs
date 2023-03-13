using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LogicFrenzyData : MonoBehaviour {

    public string filename; //the file that contains the information for this round of logic frenzy
    public string bgm;
    public string gameoverfile;
    public int lineNumber;

    //will be refreshed multiple times during one round
    public List<string> currResponses = new List<string>();
    public string opponent, sprite;
    public string question;
    public int correctResponse;
    public string failfile;
    public string successfile;
    public List<Pattern> patterns = new List<Pattern>();
    //effects?
}

[System.Serializable]
public class Pattern
{
    public string name;
    public int type;
    public float delay;
}
