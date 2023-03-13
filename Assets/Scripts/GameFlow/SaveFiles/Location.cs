using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Location {

    public string name;
    public string firstTimeFile;
    public string revisitFile;
    public string locationCompleteFile;
    public bool visited;
    public bool complete = false;
    public List<InteractableObject> objects;    //list of characters and objects in the scene
    public List<bool> objectsRequired;      //corresponds to list above, stating whether the object needs to be completed for location complete

    public Location(string name)
    {
        this.name = name;
        objects = new List<InteractableObject>();
    }

    public bool isComplete()
    {
        if (complete) return !complete; //first time location is complete complete var should be false until check, afterwards return false because dont want to replay file
        for(int i = 0; i < objects.Count; i++)
        {
            if (!objects[i].isComplete() && objectsRequired[i]) return false;   //not complete, but needs to be
        }
        complete = true;
        return complete;
    }
}
