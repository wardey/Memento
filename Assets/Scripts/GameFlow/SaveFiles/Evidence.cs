using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Evidence{
    public string name;
    public string desc;
    public string image;

    public Evidence(string name, string desc, string image)
    {
        this.name = name;
        this.desc = desc;
        this.image = image;
    }
}
