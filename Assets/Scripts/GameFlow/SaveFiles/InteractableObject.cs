using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[System.Serializable]
public class InteractableObject    //could be either a character or object in the scene, but doesn't matter, as long as it contains the right name and sprite at the right location
{
    //this section is for investigation mode, free time is below
    //conversations for this character class
    public List<Convo> convos = new List<Convo>();
    //responses for this character, if response for certain evidence not found, use default file
    public List<Response> evidenceResponse = new List<Response>();
    public bool isChar;
    public string name;
    public string sprite;
    public string badEvidenceFile;  //default file for evidence presentation if evidence isnt in evidence response
    public string examineFile;  //for objects only
    public float xmin, xmax, ymin, ymax;    //for scaling and positioning
    public bool complete;
    public bool convoComplete;
    public bool evidenceComplete;

    //for constructing characters, since the script only provides the name at the beginning
    public InteractableObject(string name)
    {
        this.name = name;
    }

    //premade characters need to have fields set later when adding to a location
    public void setChar(string sprite, float xmin, float xmax, float ymin, float ymax)
    {
        this.sprite = sprite;
        this.xmin = xmin;
        this.xmax = xmax;
        this.ymin = ymin;
        this.ymax = ymax;
    }

    //for contructing objects, not characters, might not necssarily be an evidence actually
    public InteractableObject(string name, string sprite, float xmin, float xmax, float ymin, float ymax, string file)
    {
        this.name = name;
        this.sprite = sprite;
        this.xmin = xmin;
        this.xmax = xmax;
        this.ymin = ymin;
        this.ymax = ymax;
        examineFile = file;
    }

    public void setExamineFile(string filename)
    {
        //since theres an examine file, cannot be a character
        isChar = false;
        examineFile = filename;
    }

    public void addConvo(string requirementType, string requirement, string displayName, string filename, string action = "none", int progress = 0)
    {
        Convo temp = new Convo();
        temp.requirement = requirement;
        temp.requirementType = requirementType;
        temp.action = action;
        temp.progress = progress;
        temp.displayName = displayName;
        temp.filename = filename;
        convos.Add(temp);
    }

    public void addResponse(string evidence, string filename)
    {
        Response temp = new Response();
        temp.evidence = evidence;
        temp.filename = filename;
        evidenceResponse.Add(temp);
        //objects cant have responses
        isChar = true;
    }

    public void setBadResponse(string filename)
    {
        badEvidenceFile = filename;
    }

    public bool isComplete()
    {
        if (isChar)
        {
            if (complete) return complete;
            foreach (Convo convo in convos)
            {
                if (!convo.complete) return false;
            }
            foreach (Response res in evidenceResponse)
            {
                if (!res.complete) return false;
            }
            Debug.Log(name + " complete");
            complete = true;
        }
        return complete;
    }

    //for free time section, characters will have convos based on affection, and only one will be played when player decides to spend time with said char
    public List<string> freetimeConvos = new List<string>(); //just a list of string filenames sorted in order of increasing affection
    //free time objects are the same as investigation

    public void addFreeTimeConvo(string filename)
    {
        freetimeConvos.Add(filename);
    }
}

[System.Serializable]
public class Convo
{
    //conversation with characters
    public string requirement;
    public string requirementType;  //none, affection, own, present
    public string action;   //add, update, none
    public int progress = 0; //only useful for updatetopic, to keep track of linear progression
    public string filename;
    public string displayName;
    public bool display = false;
    public bool complete = false;
}

[System.Serializable]
public class Response
{
    //character responses to presenting evidence
    public string evidence;
    public string filename;
    public bool complete = false;
}