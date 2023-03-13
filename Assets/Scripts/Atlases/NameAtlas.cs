using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NameAtlas : MonoBehaviour{    //remember to attach to initScripts
    //class that stores dictionary which matches a formal name to all of its short forms

    public static NameAtlas instance;
    public List<string> nameInfos = new List<string>();
    public Dictionary<string, List<string>> dictNames = new Dictionary<string, List<string>>();
    // Use this for initialization
    void Start()
    {
        instance = this;
        DialogueParser.instance.LoadFile("Assets/Resources/Names.txt");   //whatever the file name is, but has to be the absolute path Assets/...
        nameInfos = DialogueParser.instance.getLines();
        foreach (string line in nameInfos)
        {
            //well whatever the divider is, split the line, and put it in the dictionary under the name
            //format: formal name = [all other names that refer to the same character separated by commas]
            string[] name = line.Split('=');
            string[] shortforms = name[1].Split(',');
            dictNames.Add(name[0], new List<string>());
            foreach(string sf in shortforms)
            {
                dictNames[name[0]].Add(sf);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //returns the formal version of name if it exists, or just throw name back
    public string getName(string name)
    {
        foreach(KeyValuePair<string, List<string>> pair in dictNames)
        {
            if (pair.Value.Contains(name))
            {
                return pair.Key;
            }
        }
        return name;
    }

	// makes full name first name only
	public static string ToFirstName(string fullName)
	{
		return fullName.Split()[0];
	}
}
