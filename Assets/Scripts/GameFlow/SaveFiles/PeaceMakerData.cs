using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PeaceMakerData {
    public int lineNumber;
    public string bgm;
    public int correctOne, correctTwo;  //index of the two correct statements, order doesnt matter
    public string incorrectFile;
    public string nextMode, nextFile;
    public DedeuceChar currChar;
    public int charIndex;
    public List<Statement> statements = new List<Statement>();
    public List<DedeuceChar> dChars = new List<DedeuceChar>();  //list for holding all the characters and their corresponding statements
    //public List<Transition> transitions = new List<Transition>();

        //used for transitions
    public List<string> characters = new List<string>(new string[15]);
    public List<string> sprites = new List<string>(new string[15]);
    public List<string> allchars = new List<string>(new string[15]);
    public List<string> allSprites = new List<string>(new string[15]);
}

public class DedeuceChar
{
    public string name;
    public string emote;
    public List<Statement> statements = new List<Statement>();
}