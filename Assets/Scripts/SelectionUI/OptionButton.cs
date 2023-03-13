using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class OptionButton : MonoBehaviour {

    public Action<int> OnChoose;
    public Text choice;
    public int index;
	// Use this for initialization
	void Start () {
	
	}

    public void setChoice(string choice)
    {
        this.choice.text = choice;
    }

    public void OnClick()
    {
        OnChoose(index);
    }
}
