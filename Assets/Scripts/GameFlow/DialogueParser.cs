using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class DialogueParser : MonoBehaviour {

    public static DialogueParser instance;

    struct DialogueLine
    {
        public string name;
        public string content;
        public int pose;

        public DialogueLine(string Name, string Content, int Pose)
        {
            name = Name;
            content = Content;
            pose = Pose;
        }
    }

    //List<DialogueLine> lines;
    List<string> lines;
    private string nextFile;

    // Use this for initialization
    void Awake() {
        instance = this;
        //string file = "Assets/Dialogue/Dialogue1.txt"; //storyProgress.instance.currFile; 
        //lines = new List<DialogueLine>();
        //LoadDialogue(file);
    }

    public void LoadDialogue(string filename)
    {
        lines = new List<string>();
        string line;
        StreamReader r = new StreamReader("Assets/Dialogue/" + filename);

        using (r)
        {
            do
            {
                line = r.ReadLine();
                if (line != null)
                {
                    //string[] lineData = line.Split(';');
                    //DialogueLine lineEntry = new DialogueLine(lineData[0], lineData[1], int.Parse(lineData[2]));
                    lines.Add(line.Trim());
                }
            }
            while (line != null);
            r.Close();
        }
    }

    public void LoadFile(string filename)   //file name has to be the absolute path of the file
    {
        lines = new List<string>();
        string line;
        StreamReader r = new StreamReader(filename);

        using (r)
        {
            do
            {
                line = r.ReadLine();
                if (line != null)
                {
                    //string[] lineData = line.Split(';');
                    //DialogueLine lineEntry = new DialogueLine(lineData[0], lineData[1], int.Parse(lineData[2]));
                    lines.Add(line.Trim());
                }
            }
            while (line != null);
            r.Close();
        }
    }

    public List<string> getLines()
    {
        return lines;
    }
    /*
    public string GetName(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].name;
        }
        return "";
    }

    public string GetContent(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].content;
        }
        return "";
    }

    public int GetPose(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].pose;
        }
        return 0;
    }
    */

    // Update is called once per frame
    void Update () {
	
	}
}
