using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SaveFileButton : MonoBehaviour {

    public int index;
    public Text displayText;
    public Action<int> onSave;

    // Use this for initialization
    void Start()
    {

    }

    public void setNotInUse()
    {
        displayText.text = "Empty";
    }

    public void setInUse(string filename, int linenumber)
    {
        displayText.text = filename + ", Line: " + linenumber.ToString();
    }

    public void Onclick()
    {
        onSave(index);
    }
}
