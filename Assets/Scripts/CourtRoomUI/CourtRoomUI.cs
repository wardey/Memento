using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CourtRoomUI : MonoBehaviour
{   
    public Action<string, string> onRoundEnd;   //call setnewfile in gamecontroller 
    public static CourtRoomUI instance;
    public circleorganizer circle;
    public EvidenceMenuUI evidenceUI;   //deprecated

    public Text currStatementText;  //deprecated
    public Text currSpeaker;
    public Image characterCenter;
    public Image background;
    public GameObject loopButton;
    public GameObject EvidenceButton;
    //public GameObject prevButton;

    public CourtRoomModeData roundData;

    //for new debate 
    public List<Image> evidenceBar; //the references for the 5 evidence buttons (their image component)
    private int evidenceIndex;  //index to keep track of what the bar should display from the evidence list
    public Evidence selectedEvidence; //the current selected evidence
    public GameObject textBubblePrefab; //drag and drop prefab
    public List<GameObject> textBubbles = new List<GameObject>();   //list of all the bubbles made dynamically
    private int statementIndex;
    string currChar = "";
    int consecCounter = 0;
    private bool inSequence; //indicate if in the middle of the round
    private bool inTransition;
    private IEnumerator sequence;

    // Use this for initialization
    void Awake()
    {
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
        augmentSpriteList();
        refreshCircle();
        updateEvidenceBar();
        BGMController.instance.playSong(roundData.bgm);
        //would eventually need to add a intro, some kind of effect or something for transitioning to a new mode
        reset();
    }

    void augmentSpriteList()
    {
        for(int i = 0; i < roundData.allSprites.Count; i++)
        {
            if (roundData.allSprites[i] != null)
                roundData.allSprites[i] = roundData.allchars[i] + "_" + roundData.allSprites[i];
        }
    }

    public void init(CourtRoomModeData roundInfo)
    {
        roundData = roundInfo;
        roundData.currStatement = roundData.statements[0];
    }

    public void postDeabteVN()
    {
        reset();
    }

    //need to be modified for new debate, as a restart, run through the entire debate process 
    //make the debate a single coroutine
    void reset()
    {
        circle.onTransitionComplete += AfterTransition;

        /** automatic sequence*/
        //if (inSequence) StopCoroutine(sequence);
        
        //clear all the existing textboxes, reset the sprites to initial ones for each statement,
        for(int i = 0; i < textBubbles.Count; i++)
        {
            Destroy(textBubbles[i]);
        }
        textBubbles = new List<GameObject>();
        //start a separate coroutine (would need to keep a reference to the coroutine and stop it on a "new" reset)

        /** automatic sequence*/
        //sequence = debateSequence();    //remove this and wait for click instead
        //StartCoroutine(sequence);

        /*manual sequence*/
        //start from first statement
        statementIndex = -1;
        currChar = "";
        consecCounter = 0;
        nextCharacterSequence();
    }

    /** automatic sequence*/
    IEnumerator debateSequence()
    {
        inSequence = true;
        //for each character, store their statements so that when that character is visited again, we can re:create all their text bubbles
        //else destroy all the existing text bubbles and turn to the next character

        for (statementIndex = 0; statementIndex < roundData.statements.Count; statementIndex++)
        {
            roundData.currStatement = roundData.statements[statementIndex];

            //might need to change the sprite for the character being transitioned to
            //refresh the thing regardless, if theres no change it doesnt do anything anyways
            refreshCircle();

            //whoosh in (up to 3 if same character (middle -> top -> bot)) fade for now
            if (roundData.currStatement.character == currChar)
            {
                consecCounter++;
                //create a new text bubble based on the consecCounter
                StartCoroutine(spawnStatement(consecCounter));
            }
            else
            {
                //reset counter
                consecCounter = 0;
                //clear all existing text bubbles
                //whoosh out first
                clearBubbles();
                //do transition to character for the current statement, and wait until its done...
                StartCoroutine(spawnStatement(consecCounter));
            }

            //wait for a few seconds 
            //i guess should allow player to fast forward through this somehow, maybe give 2x,4x,8x speed, etc
            currChar = roundData.currStatement.character;
            while (inTransition)
            {
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(3); //can use a ratio/var instead
        }
        inSequence = false;
        disableEverything();
        circle.onTransitionComplete -= AfterTransition;
        onRoundEnd(roundData.loopFile, "debateVN");
        yield return null;
    }

    /* manual debate sequence*/
    //would make more sense to make the transitions per character rather than per statement here
    public void nextCharacterSequence()
    {
        if (inTransition) return;//optional, must wait for transition to end before going to next statement

        inSequence = true;
        statementIndex++;

        if (statementIndex == roundData.statements.Count)
        {
            disableEverything();
            circle.onTransitionComplete -= AfterTransition;
            onRoundEnd(roundData.loopFile, "debateVN");
        }

        roundData.currStatement = roundData.statements[statementIndex];

        //might need to change the sprite for the character being transitioned to
        //refresh the thing regardless, if theres no change it doesnt do anything anyways
        refreshCircle();

        //whoosh in (up to 3 if same character (middle -> top -> bot)) fade for now
        if (roundData.currStatement.character == currChar)
        {
            consecCounter++;
            //create a new text bubble based on the consecCounter
            StartCoroutine(spawnStatement(consecCounter));
        }
        else
        {
            //reset counter
            consecCounter = 0;
            //clear all existing text bubbles
            //whoosh out first
            clearBubbles();
            //do transition to character for the current statement, and wait until its done...
            StartCoroutine(spawnStatement(consecCounter));
        }

        currChar = roundData.currStatement.character;

        inSequence = false;
    }

    IEnumerator spawnStatement(int consecCounter)
    {
        //*******
        //add in a check to see if the statement has an additional press option, if so, activate a press button
        //that starts the press file.
        Debug.Log("creating bubble with consecCounter : " + consecCounter);
        GameObject tmp = Instantiate(textBubblePrefab, transform);
        RectTransform trans = tmp.GetComponent<RectTransform>();
        switch (consecCounter)
        {
            //based on which one, set positions/anchors and stuff
            case 0:
                trans.anchorMax = new Vector2(0.7f, 0.65f);
                trans.anchorMin = new Vector2(0.3f, 0.5f);
                break;
            case 1: //second consec statement
                trans.anchorMax = new Vector2(0.7f, 0.85f);
                trans.anchorMin = new Vector2(0.3f, 0.7f);
                break;
            case 2: //third consec statement
                trans.anchorMax = new Vector2(0.7f, 0.45f);
                trans.anchorMin = new Vector2(0.3f, 0.3f);
                break;
            default:
                break;
        }
        Text text = tmp.transform.Find("Text").GetComponent<Text>();
        text.text = roundData.currStatement.statement;
        StatementBubble script = tmp.GetComponent<StatementBubble>();
        script.statement = roundData.currStatement;
        script.onClick += onTextBoxClick;
        textBubbles.Add(tmp);
        tmp.SetActive(true);
        //make the thing come in from the left side, move and fade in one, move for now, fade is a bit hard
        /*trans.offsetMax = new Vector2(-Screen.width/2, 0f);
        trans.offsetMin = new Vector2(Screen.width/2, 0f);
        for(int i = 0; i < 100; i++)
        {
            trans.offsetMax = Vector2.Lerp(trans.offsetMax, Vector2.zero, Screen.width / 20);
            trans.offsetMin = Vector2.Lerp(trans.offsetMin, Vector2.zero, Screen.width / 20);
        }*/

        yield return null;
    }

    //check if the presented evidence is the correct one for this round of debate
    public void checkEvidence(Evidence evidence, Statement statement)
    {
        if (evidence.name == roundData.correctEvidence.name && statement.statement == roundData.correctStatement.statement)   //is it correct?
        {
            correctObject();
        }
        else
        {
            //play an incorrect file instead, no more hp system
            circle.onTransitionComplete -= AfterTransition;
            onRoundEnd(roundData.incorrectFile, "debateVN");
            disableEverything();
            //start oer again from the beginning of the debate
            reset();
            //loseHP();
        }
    }

    void correctObject()    //right objection
    {
        //end of round, exit courtroom mode i guess, back to dialogue mode, or whichever

        //clearIndicators();
        clearBubbles();
        circle.onTransitionComplete -= AfterTransition;
        onRoundEnd(roundData.nextFile, roundData.nextMode);
        gameObject.SetActive(false);    //disable debate ui before exiting
    }
    
    //updates the evidence bar with the corresponding images of the evidence they're supposed to show
    //should update the descriptions for each evidence each time the evidences are updated
    void updateEvidenceBar()
    {
        //for each button, load the corresponding sprite for the evidence its supposed to represent
        for(int i = 0; i < 5; i++)
        {
            //need to check if we've run out of evidences, so we dont get null reference exception
            //if theres no evidences left to display, the picture can just show a default one or something
            if (evidenceIndex * 5 + i >= GameController.evidenceBag.Count)
            {
                //need to disable the button as well, and disable the hovering *********
                evidenceBar[i].sprite = SpriteAtlas.instance.loadSprite(""); //there should be a defualt image
            }
            else evidenceBar[i].sprite = SpriteAtlas.instance.loadSprite(GameController.evidenceBag[evidenceIndex * 5 + i].image);
        }
        //also update the information box for each item *********
    }

    //pressing left on evidence bar, moves the index over by 1 group (5 items)
    public void eviLeft()
    {
        if (evidenceIndex > 0)
        {
            evidenceIndex--;
            updateEvidenceBar();
        }
    }

    //pressing right on evidence bar, moves the index over by 1 group(5 items)
    public void eviRight()
    {
        if(evidenceIndex + 1f < GameController.evidenceBag.Count / 5.0f)
        {
            evidenceIndex++;
            updateEvidenceBar();
        }
    }

    //for later, but when the individual evidence is clicked, something's supposed to happen
    //the index will be hard coded on to the buttons
    public void eviClicked(int index)
    {
        selectedEvidence = GameController.evidenceBag[evidenceIndex*5 + index];
        Debug.Log("selected evidence : " + selectedEvidence.name);
    }

    //when hovering over an evidence should display the information for that evidence in a floating box
    //maybe moves with the cursor but could also be static relative to the evidence box
    public void onEviHover(int index)
    {
        evidenceBar[index].transform.GetChild(0).gameObject.SetActive(true);
    }

    public void EviHoverOff(int index)
    {
        evidenceBar[index].transform.GetChild(0).gameObject.SetActive(false);
    }

    //when a textbox is clicked by the player, need to check if an evidence has been selected
    //also need the textbox to return something so that we can know which box was clicked,
    //or perhaps the statement that the textbox correspondes to for that character.
    public void onTextBoxClick(Statement statement)
    {
        Debug.Log("clicked statement: " + statement.statement);
        //if theres no selected evidence, then do nothing, or some kind of error sound
        if (selectedEvidence == null) return;
        checkEvidence(selectedEvidence, statement);
        //if there is selected, check evidence for correctness, the rest should already be done
    }

    //if the player clicks the mask (well basically anything that isnt a textbox or evidence box) "unload" the evidence
    //would need a visual change on the selected evidence box
    public void clickMask()
    {
        selectedEvidence = null;
        Debug.Log("selected evidence: " + selectedEvidence);
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

    public void AfterTransition()
    {
        //in debate-vn right now it calls this when a transition is done too, so need to deal with that
        //enableEverything();
        inTransition = false;
    }

    public void refreshCircle()
    {
        string name = NameAtlas.instance.getName(roundData.currStatement.character);
        string emote = roundData.currStatement.emote;
        roundData.allSprites[findChar(name)] = name + "_" + emote;
        repopulate(findChar(name), name);
        circle.currChar = findChar(name);
        //the transitions thing doesnt even do anything right now
        //maybe a special case for the last statement transitioning back to the beginning
        inTransition = true;
        StartCoroutine(circle.transitionToCurrChar(roundData.currStatement.transition));
    }

    //right now this isnt being called at all, enable is being called but does nothing, 
    //if we want to fade out the ui while transitioning we could use this, but it doesnt make too much sense
    //need some kind of transition effect on the ui and text, but i guess thats a minor detail for later
    public void disableEverything()
    {
        clearBubbles();
        currSpeaker.gameObject.SetActive(false);
        currStatementText.gameObject.SetActive(false);
        transform.Find("background").gameObject.SetActive(false);
        //transform.Find("Inquire").gameObject.SetActive(false);
        transform.Find("Speaker").gameObject.SetActive(false);
        //transform.Find("next").gameObject.SetActive(false);
        transform.Find("EvidenceBar").gameObject.SetActive(false);
    }

    public void enableEverything()
    {
        currSpeaker.gameObject.SetActive(true);
        currStatementText.gameObject.SetActive(true);
        transform.Find("background").gameObject.SetActive(true);
        //transform.Find("Inquire").gameObject.SetActive(true);
        transform.Find("Speaker").gameObject.SetActive(true);
        //transform.Find("next").gameObject.SetActive(true);
        transform.Find("EvidenceBar").gameObject.SetActive(true);
    }

    void clearBubbles()
    {
        for (int i = 0; i < textBubbles.Count; i++)
        {
            Destroy(textBubbles[i]);
        }
        textBubbles.Clear();
    }
}
