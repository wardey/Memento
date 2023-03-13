using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SelectionUI : MonoBehaviour {

    public Action<int> onChooseOption;
    public static SelectionUI instance;

    public List<string> choices;
    public List<OptionButton> buttons; //reuse 4 button, disable ones not used
    //public GameObject optionButton; //prefab of choice button
    //public Transform optionsParent; //parent of newly created buttons

	// Use this for initialization
	void Awake () {
        instance = this;
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].OnChoose += selectOption;
        }
        disableButtons();
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void disableButtons()
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
    }

    public void setOption(string option, int index)
    {
        buttons[index-1].setChoice(option);
        buttons[index-1].index = index;
        buttons[index-1].gameObject.SetActive(true);
    }

    public void selectOption(int index)
    {
        //do something, when an option is selected
        onChooseOption(index);
        gameObject.SetActive(false);
        disableButtons();
    }
}
