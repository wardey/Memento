using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvidenceAtlas : MonoBehaviour {    //remember to attach to initScripts
    //stores distionary of information for evidences
    //master file that contains all evidences
    //load evidence from here by name
    public static EvidenceAtlas instance;
    public List<string> evidenceInfos = new List<string>();
    public Dictionary<string, List<string>> dictEvidences = new Dictionary<string, List<string>>();
    // Use this for initialization
    void Start () {
        instance = this;
        DialogueParser.instance.LoadFile("Assets/Resources/Evidences.txt");   //whatever the file name is, but has to be the absolute path Assets/...
        evidenceInfos = DialogueParser.instance.getLines();
        foreach(string line in evidenceInfos)
        {
            //well whatever the divider is, split the line, and put it in the dictionary under the name
            //i wanna say name=desc=image
            string[] evidence = line.Split('=');
            dictEvidences.Add(evidence[0], new List<string>());
            dictEvidences[evidence[0]].Add(evidence[1]);    //desc
            dictEvidences[evidence[0]].Add(evidence[2]);    //image
        }
	}
	
	// Update is called once per frame
	void Update () {

	}

    //loads a piece of evidence by name
    public Evidence loadEvidence(string name)
    {
        Evidence tmp = new Evidence(name, dictEvidences[name][0], dictEvidences[name][1]);  //or however it's structured
        return tmp;
    }

    //loads a clue by name
/*    public Clue loadClue(string name)
    {

    }
    */
}
