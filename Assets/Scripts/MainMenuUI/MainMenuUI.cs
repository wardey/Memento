using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public GameObject menuUI;
    public GameObject optionsMenuUI;
    public GameObject loadGameUI;
    public GameObject saveMenuUI;

    public List<LoadFileButton> savefiles;
    public List<SaveFileButton> saveFileButtons;

	// Use this for initialization
	void Start () {
	    for(int i = 0; i < savefiles.Count; i++)
        {
            //if save file isnt used, disable corresponding button
            //if it is, set info and stuff for that button
            savefiles[i].onLoad += OnLoad;
            savefiles[i].index = i + 1;
            savefiles[i].setNotInUse();
        }
        for(int i = 0; i < saveFileButtons.Count; i++)
        {
            saveFileButtons[i].onSave += OnSave;
            saveFileButtons[i].index += i + 1;
            saveFileButtons[i].setNotInUse();
        }
	}

    void OnDestroy()
    {
        for (int i = 0; i < savefiles.Count; i++)
        {
            savefiles[i].onLoad -= OnLoad;
        }
    }

    //attached to savefile buttons
    public void OnLoad(int index)
    {
        Debug.Log("load savefile " + index.ToString());
        storyProgress.instance.Load(index);
        onBack();
        gameObject.SetActive(false);
    }

    public void OnSave(int index)
    {
        Debug.Log("saving to savefile " + index.ToString());
        storyProgress.instance.Save(index);
        onSaveClick();
    }

    //on clicking a specific button, something happens, no specific options yet, perhaps graphics, sound, etc...
    public void onBack()    //basically closes all sub menus
    {
        optionsMenuUI.SetActive(false);
        loadGameUI.SetActive(false);
        saveMenuUI.SetActive(false);
    }

    public void onOptionsClick()    //goes to options menu
    {
        optionsMenuUI.SetActive(true);
    }
    
    public void onSaveClick()   //goes to save menu
    {
        LoadSave.Load();
        for (int i = 0; i < LoadSave.savedGames.Count; i++)
        {
            //need to display something
         //   saveFileButtons[i].setInUse(LoadSave.savedGames[i].File, LoadSave.savedGames[i].lineNumber);
        }
        saveMenuUI.SetActive(true);
    }

    public void onLoadClick()   //goes to load menu
    {
        LoadSave.Load();
        for (int i = 0; i < LoadSave.savedGames.Count; i++)
        {
            //need to display something
       //     savefiles[i].setInUse(LoadSave.savedGames[i].File, LoadSave.savedGames[i].lineNumber);
        }
        loadGameUI.SetActive(true);
    }

    public void onNewGame()
    {
        storyProgress.instance.newGame();
        gameObject.SetActive(false);
        //do something...
    }

    public void onExit()
    {
        Application.Quit();
    }
}
