using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PeaceMakerUI : MonoBehaviour {

    public Action<string, string> onRoundEnd;   //call setnewfile in gamecontroller 
    public static PeaceMakerUI instance;
    public circleorganizer circle;

    public PeaceMakerData roundData;
    //declare ui variables here, and other related variables for the mode
    private bool inTransition;
    private Statement s1, s2;
    public GameObject select1, select2; //the selected statements
    public GameObject statement1, statement2;   //the displayed statements, up to max of 2 per character
    private List<GameObject> textBoxes;
    
    //basically, set up circle once, then just go to the character when they press left or right
    //the order of the characters will be initially stated using a keyword
    //everytime go to a character, clear the statement boxes, just reset the statement box text and do some fading or stuff
    //on click statement box, put it into the selection box
    //on click selection box, empty it if theres something in it
    //on present, check the two selected statemnets, play incorrect file if wrong, and clear the two selections

    void Awake () {
        instance = this;
        //evidenceUI.PresentCheck += checkEvidence;
        circle = GameObject.Find("CircleCenter").GetComponent<circleorganizer>();
        circle.debateCamera.enabled = false;
        gameObject.SetActive(false);
    }

    //need to change the circle to be more like mirai nikki?
    public void startRound()    //after gamecontroller finishes reading the file and setting information, starts debate round
    {
        gameObject.SetActive(true); //enable debate ui
        //need to enable the camera too for the circle
        circle.debateCamera.enabled = true;
        circle.onTransitionComplete += AfterTransition;
        //initial circle setup, dont make any changes after to it after
        augmentSpriteList();
        //set up the initial variables before this
        //init to the first character
        roundData.currChar = roundData.dChars[0];
        roundData.charIndex = 0;
        clearSelection(1);
        clearSelection(2);
        oncePop = true;
        //refreshCircle();
        
        //bgm keywoprd not working, so temporarily disable this lineS
        BGMController.instance.playSong(roundData.bgm);
        
        //refresh ui  
        updateStatements();
    }

    void augmentSpriteList()
    {
        for (int i = 0; i < roundData.allSprites.Count; i++)
        {
            if (roundData.allSprites[i] != null)
                roundData.allSprites[i] = roundData.allchars[i] + "_" + roundData.allSprites[i];
        }
    }

    public void init(PeaceMakerData roundInfo)
    {
        roundData = roundInfo;
    }

    //before a transition
    public void updateStatements()
    {
        //make the ui changes after setting variables
        statement1.SetActive(false);
        statement2.SetActive(false);
        //transition to current char
        refreshCircle();

        //do transition
    }
    
    /*** button functions ***/
    //move one character to tthe right
    public void nextRight()
    {
        roundData.charIndex++;
        if (roundData.charIndex == roundData.dChars.Count) roundData.charIndex = 0;
        roundData.currChar = roundData.dChars[roundData.charIndex];
        updateStatements();
    }

    //move one character to the left
    public void nextLeft()
    {
        roundData.charIndex--;
        if (roundData.charIndex == 0) roundData.charIndex = roundData.dChars.Count - 1;
        roundData.currChar = roundData.dChars[roundData.charIndex];
        updateStatements();
    }

    //clear one of the two selected statements by clicking on the button representing the statement
    //deactivates the present button
    public void clearSelection(int index)
    {
        if (index == 1)
        {
            s1 = null;
            //unset the selected statement visually
            select1.GetComponentInChildren<Text>().text = "";
        }
        if (index == 2)
        {
            s2 = null;
            //unset the selected statement visually
            select2.GetComponentInChildren<Text>().text = "";
        }
        transform.Find("Present").gameObject.SetActive(false);
    }   

    //click on textbox for staetments to select one and put it into the bar on the menu
    //only activates the present button if both selections are filled
    public void selectStatement(int index)
    {
        //also cant let them select the same statement
        //put it into an empty box
        if (s1 == roundData.currChar.statements[index - 1] || s2 == roundData.currChar.statements[index - 1]) return;
        if(s1 == null)
        {
            s1 = roundData.currChar.statements[index - 1];
            select1.GetComponentInChildren<Text>().text = s1.statement;
        }
        else if(s2 == null)
        {
            s2 = roundData.currChar.statements[index - 1];
            select2.GetComponentInChildren<Text>().text = s2.statement;
        }
        if(s1 != null & s2 != null) transform.Find("Present").gameObject.SetActive(true);
    }

    // selected 2 statements, present the choices
    public void onPresent()
    {
        circle.onTransitionComplete -= AfterTransition;
        if (s1 == roundData.statements[roundData.correctOne] && s2 == roundData.statements[roundData.correctTwo] ||
           s1 == roundData.statements[roundData.correctTwo] && s2 == roundData.statements[roundData.correctOne])
        {
            onRoundEnd(roundData.nextFile, roundData.nextMode);
        }
        else
        {
            disableEverything();
            onRoundEnd(roundData.incorrectFile, "debateVN");
            //do something here, maybe unload both statements when they get it wrong, or do some kind of effect
        }
    }

    public void postDebateVN()
    {
        //everything after coming back from a debatevn, 
        //so repopulate the circle
        //reset to first character,
        //clear statements and boxes (should be done already) as part of update statements
        //startRound();
        Debug.Log("post debate vn for dedeuce");
        circle.onTransitionComplete += AfterTransition;
        roundData.currChar = roundData.dChars[0];
        roundData.charIndex = 0;
        clearSelection(1);
        clearSelection(2);
        oncePop = true;
        BGMController.instance.playSong(roundData.bgm);
        updateStatements();
    }

    public void disableEverything()
    {
        statement1.SetActive(false);
        statement2.SetActive(false);
        transform.Find("selectBar").gameObject.SetActive(false);
        transform.Find("Left").gameObject.SetActive(false);
        transform.Find("Right").gameObject.SetActive(false);
        transform.Find("Present").gameObject.SetActive(false);
    }

    public void enableEverything()
    {
        statement1.SetActive(true);
        statement2.SetActive(true);
        transform.Find("selectBar").gameObject.SetActive(true);
        transform.Find("Left").gameObject.SetActive(true);
        transform.Find("Right").gameObject.SetActive(true);
        transform.Find("Present").gameObject.SetActive(false);
    }
    
    //***                               ***
    //*** functions for the 3d circle.  ***
    //***                               ***
    int findChar(string name)
    {
        //doesnt work when there are duplicate names, ie same character has multiple statements
        int i;
        for (i = 0; i < 15; i++)
        {
            if (roundData.allchars[i] == name) break;
        }
        return i;
    }

    //need to change this since not 15 characters anymore
    public void repopulate(int startindex, string focusChar)
    {
        Debug.Log("startindex: " + startindex);
        int i = startindex;
        int k = 0;
        for (int j = 0; j <= 7; j++)
        {
            if (i == 15) i = 0;
            while (roundData.allchars[i] == "" || roundData.allchars[i] == null)
            {
                if (i == 14) i = 0;
                else
                    i++;
            }
            k = (startindex + j) % 15;
            roundData.sprites[k] = roundData.allSprites[i];
            roundData.characters[k] = roundData.allchars[i];
            //Debug.Log(k + " " + roundData.characters[k] + " " + roundData.sprites[k]);
            i++;
        }
        i = startindex - 1;
        if (i == -1) i = roundData.allchars.Count - 1;
        for (int j = 14; j > 7; j--)
        {
            if (i == -1) i = 14;
            while (roundData.allchars[i] == "" || roundData.allchars[i] == null)
            {
                if (i == 0) i = 14;
                else
                    i--;
            }
            k = (startindex + j) % 15;
            roundData.sprites[k] = roundData.allSprites[i];
            roundData.characters[k] = roundData.allchars[i];
            //Debug.Log(k + " " + roundData.characters[k] + " " + roundData.sprites[k]);
            i--;
        }
        //need to find all the corresponding sprites for those characters
        circle.repopulateWithSprites(roundData.characters, roundData.sprites);
    }

    //after transition is over
    public void AfterTransition()
    {
        //in debate-vn right now it calls this when a transition is done too, so need to deal with that
        //enableEverything();
        inTransition = false;

        //update for curr char
        statement1.GetComponentInChildren<Text>().text = roundData.currChar.statements[0].statement;
        statement1.SetActive(true);
        if (roundData.currChar.statements.Count == 2)
        {
            statement2.GetComponentInChildren<Text>().text = roundData.currChar.statements[1].statement;
            statement2.SetActive(true);
        }
        else
        {
            statement2.GetComponentInChildren<Text>().text = "";
            statement2.SetActive(false);
        }
    }

    private bool oncePop;

    public void refreshCircle()
    {
        string name = NameAtlas.instance.getName(roundData.currChar.name);
        string emote = roundData.currChar.emote;
        roundData.allSprites[findChar(name)] = name + "_" + emote;
        if (oncePop)
        {
            repopulate(findChar(name), name);
            oncePop = false;
        }
        circle.currChar = findChar(name);
        //the transitions thing doesnt even do anything right now
        //maybe a special case for the last statement transitioning back to the beginning
        inTransition = true;
        StartCoroutine(circle.transitionToCurrChar(new Transition("dedeucePreset")));
        //always use the same transition, turning one character to the left or right
    }
}
