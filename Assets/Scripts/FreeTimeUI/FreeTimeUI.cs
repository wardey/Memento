using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FreeTimeUI : MonoBehaviour {

    public static FreeTimeUI instance;
    public FreeTimeModeData currData;
    public List<InteractableObject> chars;  //characters, only for construction purposes, not actually necessary to save, need to clear at end of round
    public Action<string, string> onRoundEnd;   //subscribed to by controller, plays file
    public Action talkConfirm;

    public Image background;    //background for locations, also works as the parent for all objects
    public GameObject interactableObject;   //prefab for interactable objects
    public List<FreeTimeInteractableObjectUI> currSceneObjs;    //objects in the current location, clear and destroy when switching location

    //interaction menu is done independently with the object
    //probably need to make buttons for travel and do the enable disable thing
    public GameObject travelMenu;
    public List<ConvoButton> travelButtons;
    public GameObject confirmMenu;
    public Text confirmDesc;

    private bool shortCircuit;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        for (int i = 0; i < travelButtons.Count; i++)
        {
            travelButtons[i].clickAction += onTravel;
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public InteractableObject findChar(string name) //finds the character with name in chars
    {
        int i;
        for (i = 0; i < chars.Count; i++)
        {
            if (chars[i].name == name)
            {
                break;
            }
        }
        return chars[i];
    }

    public Location findLoc(string name)
    {
        int i;
        for (i = 0; i < currData.scenes.Count; i++)
        {
            if (currData.scenes[i].name == name)
            {
                break;
            }
        }
        return currData.scenes[i];
    }
    /*
    InteractableObject findObj(string name) //return reference to the obj with matching name, assumes it exists, could be in any location
    {
        int length = currData.scenes.Count;
        for (int i = 0; i < length; i++)
        {
            int count = currData.scenes[i].objects.Count;
            List<InteractableObject> temp = currData.scenes[i].objects;
            for (int j = 0; j < count; j++)
            {
                if(temp[j].name == name)
                {
                    return 
                }
            }
        }
    }*/

    public void startRound()
    {
        //set up first scene and play starting dialogue
        currData.currLocation = currData.startLocation;
        setUp(currData.currLocation);
        //set up travel buttons with locations
        refreshTravel();
        gameObject.SetActive(true);
        if(currData.startScript != "")
            onRoundEnd(currData.startScript, "VN");
        findLoc(currData.currLocation).visited = true;
    }

    public void endRound()  //ends after playing any character file
    {
        //play some effect or something
        for (int i = 0; i < currSceneObjs.Count; i++)
        {
            Destroy(currSceneObjs[i].gameObject);
        }
        currSceneObjs.Clear();
        chars.Clear();
        currData.scenes.Clear();
        shortCircuit = false;
        gameObject.SetActive(false);
        onRoundEnd(currData.nextFile, currData.nextMode);
    }

    public void onInteract(string filename, bool isChar)  //when an object is interacted by player, object will determine which file to play and return to ui
    {
        //play the file, always going to be 
        //if the interaction is with a character, then the round is over, so shortcircuit will be true
        shortCircuit = isChar;
        onRoundEnd(filename, "VN");
    }

    public void afterInteract() //will be called after a dialogue file is complete, after return keyword from gamecontroller
    {
        if (shortCircuit)
        {
            endRound(); //shortcircuits the round if the file has been played
        }
    }

    public void onConfirm()
    {
        talkConfirm();  //something will have subscribed to this action when it is called
    }

    public void OnCancel()
    {
        confirmMenu.SetActive(false);
        talkConfirm -= talkConfirm;
    }

    public void openTravel()    //opens travel menu on button click  
    {
        //displays all locations available except current one

        travelMenu.SetActive(true);
    }

    public void refreshTravel()
    {
        int i, j = 0;
        for (i = 0; i < travelButtons.Count; i++)
        {
            travelButtons[i].gameObject.SetActive(false);
        }
        for (i = 0; i < currData.scenes.Count; i++)
        {
            if (currData.scenes[i].name != currData.currLocation)
            {
                travelButtons[j].setTopic(currData.scenes[i].name);
                travelButtons[j].gameObject.SetActive(true);
                j++;
            }
        }
    }

    public void onTravel(string name)  //after clicking a certain location, reset display to new location
    {
        //call setup on new location
        //can just use convo buttons, and the topic as the name of location
        //clear the objects in the current scene first, set up again after travel
        for (int i = 0; i < currSceneObjs.Count; i++)
        {
            Destroy(currSceneObjs[i].gameObject);
        }
        currSceneObjs.Clear();
        currData.currLocation = name;
        setUp(currData.currLocation);
        refreshTravel();
        Location tmp = findLoc(currData.currLocation);
        //onRoundEnd(tmp.visited ? tmp.revisitFile : tmp.firstTimeFile, "VN");
        tmp.visited = true;
    }

    public void setUp(string location)  //given the reference to the background object, set up all all the objects in this location
    {
        //for each interactable object, create a prefab with according to the object's specifications
        //make a interacttable objectui class that handles all ui for the prefab, with a interactable object field, give the prefab the corresponding 
        //interactable object class when constructing it
        //list of interactableobjectui's that hold the ones for the current location, clear it and re setup when traveling to a different location
        Location currLocation = findLoc(currData.currLocation);
        int length = currLocation.objects.Count;
        for (int i = 0; i < length; i++)
        {
            //make a prefab, set its parent to the background, and initialize it with a interactable object from the current location, add it to the list
            GameObject tmp = Instantiate(interactableObject);
            tmp.transform.SetParent(background.gameObject.transform);
            FreeTimeInteractableObjectUI temp = tmp.GetComponent<FreeTimeInteractableObjectUI>();
            temp.init(currLocation.objects[i]);
            currSceneObjs.Add(temp);
            tmp.SetActive(true);
        }
        background.sprite = SpriteAtlas.instance.loadSprite(currLocation.name);
    }

    public void travelHoverOff()
    {
        travelMenu.SetActive(false);
    }
}
