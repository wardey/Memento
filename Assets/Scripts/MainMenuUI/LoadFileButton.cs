using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class LoadFileButton : MonoBehaviour {

    public int index;
    public Text displayText;
    public Action<int> onLoad;

	// Use this for initialization
	void Start () {
	    
	}

    public void setNotInUse()
    {
        gameObject.GetComponent<Button>().interactable = false;
        displayText.text = "Empty";
    }

    public void setInUse(string filename, int linenumber)
    {
        gameObject.GetComponent<Button>().interactable = true;
        displayText.text = filename + ", Line: " + linenumber.ToString();
    }

    public void Onclick()
    {
        onLoad(index);
    }
}
