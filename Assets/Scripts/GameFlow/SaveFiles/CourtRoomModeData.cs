using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CourtRoomModeData {
    //data specific to courtroom/debate mode
    public int lineNumber;
    public Statement currStatement;
    public Statement correctStatement;
    public Evidence correctEvidence;
    public string incorrectFile, loopFile;
    public int hp;
    public string nextFile, nextMode;
    public string bgm;
    public List<Statement> statements = new List<Statement>();
    public List<Transition> transitions = new List<Transition>();
    public List<string> characters = new List<string>(new string[15]);
    public List<string> sprites = new List<string>(new string[15]);
    public List<string> allchars = new List<string>(new string[15]);
    public List<string> allSprites = new List<string>(new string[15]);
}
