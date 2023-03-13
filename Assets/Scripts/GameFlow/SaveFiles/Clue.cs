using UnityEngine;
using System.Collections;

[System.Serializable]
public class Clue {
    public string name;
    public string image;
    public string desc;

    public Clue(string name, string image, string desc)
    {
        this.name = name;
        this.image = image;
        this.desc = desc;
    }
}
