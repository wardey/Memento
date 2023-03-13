using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//script should be attached to logic frenzy's ui, and be enabled/disabled from game controller as well

/// <summary>
/// currently need to work on this script, gamecontroller's logic frenzy section is done for the most part
/// need to do logic frenzy's ui stuff and change the actual gameplay as well
///
/// </summary>
public class LogicFrenzyController : MonoBehaviour
{
    public static LogicFrenzyController instance;
    public Action clickDialogue;
    public Action<string, string> OnRoundEnd; //used to get game controller to read new files
    //should be responsible for most of the mode, but gamecontroller will still read the file for the round
    //and give input to this class
    public LogicFrenzyData currData;
    public BHPlayerController player;
    public EnemyController enemy;
    public Camera logicFrenzyCamera;

    //text display stuffs
    public Text dialogue;
    private float letterDelay = 0.01f;
    private string str;
    private bool dialogueDisplayDone = false;
    private bool skipCurrDialogue = false;
    IEnumerator textDisplay;

    public Image leftChar;
    public Text leftCharName;
    public Image rightChar;
    public Text rightCharName;
    public Text question;
    public List<Text> responses = new List<Text>();
    public Text hpDisplay;
    public GameObject options;
    public GameObject pressA;
    //Boundary(-4.5f, 4.5f, 0f, 7f), spawning the four options to choose from has to be within this boundary, also reasonably apart but not too close to the edge as well
    //center is (-272, -207, 3.5)
    //just gonna dictate 4 positions for the spawns instead of random

    void Awake()
    {
        instance = this;
        logicFrenzyCamera.enabled = false;
        foreach (Text t in responses) t.gameObject.SetActive(false);
        options.SetActive(false);
        gameObject.SetActive(false);    //all UI's disable themselves at the beginning, to be reenabled by game controller
        pressA.SetActive(false);
        enemy.initialPos = enemy.transform.position;
        player.initialPos = player.transform.position;
        takeControls();
    }

    public void reset()
    {
        takeControls();
        logicFrenzyCamera.enabled = false;
        foreach (Text t in responses) t.gameObject.SetActive(false);
        options.SetActive(false);
        pressA.SetActive(false);
    }

    public void startRound()
    {
        //initializes all the information then start one round with the given information
        displayLine("");
        waitingForClick = false;
        StartCoroutine(OneRound());
    }

    public void takeControls()
    {
        player.setPlayerControl(false);
        //"pause" the enemy, center it and make it stop coroutines
        enemy.center();
        player.center();
    }

    public void returnControls()
    {
        player.setPlayerControl(true);
        enemy.active = true;
        //starts a new round essentially
    }

    //plays through all the patterns indicated in the file and if player survives until the end then the round is over
    //refered to as a "battle", the whole mode is one round
    IEnumerator OneRound()
    {
        //play some kind of effect for the beginning of round
        //set the question for the battle
        question.text = currData.question;
        //instantiate the 4 options
        options.SetActive(true);
        //setup the 4 option texts and enable them
        for(int i = 0; i < currData.currResponses.Count; i++)
        {
            responses[i].text = currData.currResponses[i];
            responses[i].gameObject.SetActive(true);
        }
        //return controls back to player
        returnControls();
        //play through the patterns, if reach the end of the list, then round is over
        for(int i = 0; i < currData.patterns.Count; i++)
        {
            Pattern tmp = currData.patterns[i];
            //currently supports : targetburst, lspiral, rspiral, fspiral, semicircle, fan, irelia
            //if statements for the type of pattern and call them in enemy controller
            if(tmp.name == "targetburst")
            {
                enemy.targetBurst(tmp.type);
            }
            else if (tmp.name == "lspiral")
            {
                enemy.leftspiral(tmp.type);
            }
            else if (tmp.name == "rspiral")
            {
                enemy.rightspiral(tmp.type);
            }
            else if (tmp.name == "fspiral")
            {
                enemy.fullspiral(tmp.type);
            }
            else if (tmp.name == "semicircle")
            {
                enemy.semicircle(tmp.type);
            }
            else if (tmp.name == "fan")
            {
                enemy.fan(tmp.type);
            }
            else if(tmp.name == "irelia")
            {
                enemy.irelia();
            }
            yield return new WaitForSeconds(tmp.delay);
        }
        //automatically fail here if player did not pick a choice
        OnRoundEnd(currData.failfile, "logicfrenzydialogue");
        yield return null;
    }

    //visual representation of losing hp/getting hit
    public void loseHP()
    {
        //lose a heart or something
        hpDisplay.text = player.hp.ToString();
    }

    //whatever happens when player dies
    public void zeroHP()
    {
        StopAllCoroutines();
        enemy.StopAllCoroutines();
        player.StopAllCoroutines();
        takeControls();
        //play the game over file, which will consequently call the round file again
        OnRoundEnd(currData.gameoverfile, "VN");
        player.hp = 10;
    }

    public void setOptionText(int index, bool show)
    {
        Debug.Log("setting option text");
        dialogue.text = show ? "option " + index.ToString() : "";//currData.currResponses[index] : "";
        pressA.SetActive(show);
    }

    public void onSelect(int choice)
    {
        //probably play some kind of effect here for the selection
        //deactivate the options when one is picked
        //foreach (Text t in responses) t.gameObject.SetActive(false);
        options.SetActive(false);
        StopAllCoroutines();
        //enemy.StopAllCoroutines();
        //player.StopAllCoroutines();
        takeControls();
        if (choice == currData.correctResponse)
        {
            //correct, move on to next round if there is one, or the entire thing is over
            player.hp = 10;
            clickDialogue();    //not what this was intended for, but it serves the same purpose, go to gamecontroller's logicfrenzy() for it to keep reading in the next round
        }
        else
        {
            //incorrect
            //play file with the mode set to "logicfrenzydialogue"
            //the fail file will start the whole round over again, its next will point to the original LF file
            OnRoundEnd(currData.failfile, "logicfrenzydialogue");
            //start same round again
            //but harder, who knows what harder is though
            player.hp = 10;
            increaseDifficulty();
            //startRound();
        }
    }

    void increaseDifficulty()
    {
        //not sure what this means yet
    }

    public void setChar(string name, string emote, string pos)
    {
        if(pos == "l")
        {
            leftChar.sprite = SpriteAtlas.instance.loadSprite(name + "_" + emote);
            leftCharName.text = NameAtlas.instance.getName(name);
        }
        else if(pos == "r")
        {
            rightChar.sprite = SpriteAtlas.instance.loadSprite(name + "_" + emote);
            rightCharName.text = NameAtlas.instance.getName(name);
        }
    }

    bool waitingForClick = false;

    /////////////////////////////////////////
    ////// dialogue display section /////////
    /////////////////////////////////////////
    //display the line in logic frenzy's dialogue box and wait for click to call logic frenzy in gamecontroller again
    public void displayLine(string line)    //displays line in dialogue box
    {
        textDisplay = AnimateText(preprocess(line));
        StartCoroutine(textDisplay);
        waitingForClick = true;
    }

    public void onClick()
    {
        if (waitingForClick)
        {
            waitingForClick = false;
            clickDialogue();//same as calling logicfrenzy() in game controller
        }
    }

    string preprocess(string line)  //change all the text highlight to match the unity markup syntax
    {
        //<colour:text> -> <color=colour>text</color>
        int counter = 0;
        string newLine = "";
        while (counter < line.Length)
        {
            if (line[counter] == '<')
            {
                counter++;
                newLine += "<color=";
                while (line[counter] != ':')
                {
                    newLine += line[counter];
                    counter++;
                }
                newLine += ">";
                counter++;  //skips the :
                while (line[counter] != '>')
                {
                    newLine += line[counter];
                    counter++;
                }
                newLine += "</color>";
                counter++; //skips the >
            }
            else
            {
                newLine += line[counter];
                counter++;
            }
        }
        return newLine;
    }

    IEnumerator AnimateText(string strComplete)         //effect for making dialogue appear one by one
    {
        dialogueDisplayDone = false;
        int i = 0, j;
        str = "";
        while (i < strComplete.Length)
        {
            if (skipCurrDialogue)
            {
                dialogue.text = strComplete;
                skipCurrDialogue = false;
                dialogueDisplayDone = true;
                yield break;
            }
            if (strComplete[i] == '<') //start of markup
            {
                //preprocess #2
                while (strComplete[i] != '>')   //first half of tag
                {
                    str += strComplete[i];
                    i++;
                }
                str += ">";
                i++;
                j = i;  //start of text within markup
                while (strComplete[i] != '<')    //skip text
                {
                    i++;
                }
                while (strComplete[i] != '>')   //second half of tag
                {
                    str += strComplete[i];
                    i++;
                }
                str += ">";
                i++;    //i is now after the colored section
                //j needs to insert the text into <color=color>text</color>, j is currently at the second < for str, and start of text for strComplete     
                //actual text appears on screen
                while (strComplete[j] != '<')
                {
                    str = str.Insert(j, strComplete[j].ToString());
                    j++;
                    dialogue.text = str;
                    yield return new WaitForSeconds(letterDelay);
                }
            }
            if (i < strComplete.Length) str += strComplete[i++];
            dialogue.text = str;
            yield return new WaitForSeconds(letterDelay);
        }
        skipCurrDialogue = false;
        dialogueDisplayDone = true;
    }
}