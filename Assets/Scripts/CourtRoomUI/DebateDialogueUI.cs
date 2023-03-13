using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class DebateDialogueUI : MonoBehaviour
{

    public static DebateDialogueUI instance;
    public Action onEffectFinish;
    public Action onClickDialogue;

    //text box display stuffs
    public GameObject arrow;
    public Text dialogue;
    private float letterDelay = 0.01f;
    private string str;
    private bool dialogueDisplayDone = true;
    private bool skipCurrDialogue = false;
    IEnumerator textDisplay;
    Color32 skyBlue = new Color32(133, 206, 250, 255);

    //character display
    public List<string> characters;
    public List<string> allchars;
    public Text speaker;
    public List<string> sprites;
    public List<string> allSprites;

    public circleorganizer circle;
    private bool midTransition;

    public Image item;

    public EvidenceMenuUI evidenceUI;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        evidenceUI = transform.Find("EvidenceMenuUI").GetComponent<EvidenceMenuUI>();
        characters = new List<string>(new string[15]);
        allchars = new List<string>(new string[15]);
        sprites = new List<string>(new string[15]);
        allSprites = new List<string>(new string[15]);
        circle = GameObject.Find("CircleCenter").GetComponent<circleorganizer>();
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        circle.onTransitionComplete += AfterTransition;
    }

    void OnDisable()
    {
        circle.onTransitionComplete -= AfterTransition;
    }

    public void OnClickDialogue()
    {
        if (midTransition)
        {
            Debug.Log("stilll in transition");
            return;
        }
        if (dialogueDisplayDone == false)
        {
            //stuck in this case for some reason
            Debug.Log("skip display");
            skipCurrDialogue = true;
            //StopCoroutine(textDisplay);
        }
        else
        {
            //update content from dialogueparser class
            //displayLine();
            //tell game controller that the player clicked, and to go to the next line
            if (!midTransition)
            {
                //if this gets called, will be stuck in infinite loop, not sure why either
                Debug.Log("going to next line");
                onClickDialogue();
            }
        }
    }

    int findChar(string name)
    {
        int i;
        for(i = 0; i < 15; i++)
        {
            if (allchars[i] == name) break;
        }
        return i;
    }

    public void repopulate(int startindex, string focusChar)
    {
        Debug.Log("startindex: " + startindex);
        int i = startindex;
        int k = 0;
        for (int j = 0; j <= 7; j++)
        {
            if (i == 15) i = 0;
            while (allchars[i] == "" || allchars[i] == null)
            {
                if (i == 14) i = 0;
                else
                    i++;
            }
            k = (startindex + j) % 15;
            sprites[k] = allSprites[i];
            characters[k] = allchars[i];
            //Debug.Log(k + " " + roundData.characters[k] + " " + roundData.sprites[k]);
            i++;
        }
        i = startindex - 1;
        if (i == -1) i = allchars.Count - 1;
        for (int j = 14; j > 7; j--)
        {
            if (i == -1) i = 14;
            while (allchars[i] == "" || allchars[i] == null)
            {
                if (i == 0) i = 14;
                else
                    i--;
            }
            k = (startindex + j) % 15;
            sprites[k] = allSprites[i];
            characters[k] = allchars[i];
            //Debug.Log(k + " " + roundData.characters[k] + " " + roundData.sprites[k]);
            i--;
        }
        //need to find all the corresponding sprites for those characters
        circle.repopulateWithSprites(characters, sprites);
    }

    public void refreshCircle()
    {
        repopulate(0, allchars[0]);
        circle.currChar = findChar(allchars[0]);
        //the transitions thing doesnt even do anything right now
    }

    void augmentSpriteList()
    {
        for (int i = 0; i < allSprites.Count; i++)
        {
            if (allSprites[i] != null)
                allSprites[i] = allchars[i] + "_" + allSprites[i];
        }
    }

    public void changeSpeaker(string name)  //only change speaker text
    {
        speaker.text = NameAtlas.instance.getName(name);
    }

    //doesnt actually change the speaker, only which character is in the center of the screen
    public void transition(string name, string emote, Transition transition)    //if need to do a transition to the next character
    {
        string currChar = NameAtlas.instance.getName(name);
        speaker.text = currChar;
        //find the character in the circle, repopulate, and do the transition from the current character
        allSprites[findChar(currChar)] = currChar + "_" + emote;
        repopulate(findChar(currChar), currChar);
        //after the transition coroutine is done, call back to readCommand in gamecontroller
        circle.currChar = findChar(currChar);
        StartCoroutine(circle.transitionToCurrChar(transition));
        midTransition = true;
    }

    //both of the two below need to update the actual sprites in the scene as well
    //not sure if repopulate would work, or need to write a specific method for it

    //changes speaker and the emote, keyword t=char=emote
    public void changeSpeaker(string name, string emote)
    {
        string currChar = NameAtlas.instance.getName(name);
        int index = findChar(currChar);
        allSprites[index] = currChar + "_" + emote;
        for (int i = 0; i < 15; i++)
        {
            //names same but different emotes
            //update the dynamic sprite list, and then send it over to circle to update
            if (sprites[i] == null) ;
            else if(sprites[i].Split('_')[0] == allSprites[index].Split('_')[0])
            {
                sprites[i] = allSprites[index];
            }
        }
        circle.repopulateWithSprites(sprites);
    }

    //change the sprite for the current speaker, keyword e=emote
    public void changeSprite(string emote)
    {
        int index = findChar(speaker.text);
        allSprites[index] = speaker.text + "_" + emote;
        for (int i = 0; i < 15; i++)
        {
            //names same but different emotes
            //update the dynamic sprite list, and then send it over to circle to update
            if (sprites[i] == null) ;
            else if (sprites[i].Split('_')[0] == allSprites[index].Split('_')[0])
            {
                sprites[i] = allSprites[index];
            }
        }
        circle.repopulateWithSprites(sprites);
    }

    public void AfterTransition()
    {
        Debug.Log("Debate dialogue's after transition is called");
        midTransition = false;
        OnClickDialogue();  //same effect i think, because need to call back to game controller after transition is done, similar to clicking dialogue
    }

    public void clearAll()
    {
        str = "";
        dialogue.text = str;
        dialogueDisplayDone = true;
        skipCurrDialogue = false;
        speaker.text = "";
    }

    public void showItem(string name)//displays an item
    {
        StartCoroutine(fadeeffect(item));
        item.sprite = SpriteAtlas.instance.loadSprite(name);
    }

    public void unshowItem()//fade out the current item shown smoothly, assumes that an item is presently on the screen
    {
        StartCoroutine(fadeeffect(item));
        item.sprite = SpriteAtlas.instance.loadSprite("");
    }

    public void presentprompt(string statement, string evidence)
    {
        evidenceUI.gameObject.SetActive(true);
        evidenceUI.presentprompt(statement, evidence);
    }

    //probably need some kind of effect for moving text, like text animations

    //IEnumerator section
    //for visual effects of all sorts used by dialogue ui
    //
    //

    IEnumerator fadeeffect(Image img)        //fade effect for sprites
    {
        float fade = 0f;
        float startTime;
        Color spriteColor = new Color(1f, 1f, 1f, 1f);
        while (true)
        {
            startTime = Time.time;
            while (fade > 0f)
            {
                fade = Mathf.Lerp(1f, 0f, (Time.time - startTime) / 0.3f);
                spriteColor.a = fade;
                img.color = spriteColor;
                yield return null;
            }
            fade = 0f;
            spriteColor.a = fade;
            img.color = spriteColor;

            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / 0.3f);
                spriteColor.a = fade;
                img.color = spriteColor;
                yield return null;
            }
            //Make sure it's set to exactly 1f
            fade = 1f;
            spriteColor.a = fade;
            img.color = spriteColor;
            yield break;
        }
    }

    public void displayLine(string line)    //displays line in dialogue box
    {
        dialogue.color = Color.white;
        arrow.SetActive(false);
        textDisplay = AnimateText(preprocess(line));
        StartCoroutine(textDisplay);
    }

    public void displayThought(string line)    //displays line in dialogue box
    {
        dialogue.color = (Color)skyBlue;
        arrow.SetActive(false);
        textDisplay = AnimateText(preprocess(line));
        StartCoroutine(textDisplay);
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
        arrow.SetActive(true);
    }

    //
    //section for ui effects that affects whole screen, or more than 2 characters
    //
    //effects only applicable to one character on the screen will be in the characterImage class
    //

    public Image windowCover;
    public Image noWindowCover;
    private float effectTimer = 1.0f;   //only used for effects

    public void playEffect(string fnName, float time)   //fnName has to match one of the ienumerators that are visual effects
    {
        effectTimer = time;
        StartCoroutine(fnName);
    }

    //everyone below has to call onEffectFinish at the end and run for effectTimer amount of time
    //fades to black
    IEnumerator fade()
    {
        float fade = 0f;
        float startTime;
        Color spriteColor = new Color(0f, 0f, 0f, 0f);  //black
        windowCover.color = spriteColor;
        while (true)
        {
            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / effectTimer);
                spriteColor.a = fade;
                windowCover.color = spriteColor;
                yield return null;
            }
            fade = 1f;
            spriteColor.a = fade;
            windowCover.color = spriteColor;
            onEffectFinish();
            yield break;
        }
    }
    //fades to black, but leaving textbox
    IEnumerator fade_no_window()
    {
        float fade = 0f;
        float startTime;
        Color spriteColor = new Color(0f, 0f, 0f, 0f);  //black
        noWindowCover.color = spriteColor;
        while (true)
        {
            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / effectTimer);
                spriteColor.a = fade;
                noWindowCover.color = spriteColor;
                yield return null;
            }
            fade = 1f;
            spriteColor.a = fade;
            noWindowCover.color = spriteColor;
            onEffectFinish();
            yield break;
        }
    }
    //flash white and slowly fades away
    IEnumerator flash_window()
    {
        float fade = 0f;
        float startTime;
        Color spriteColor = new Color(1f, 1f, 1f, 0f);  //white
        windowCover.color = spriteColor;
        while (true)
        {
            //sudden flash
            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / 0.1f);
                spriteColor.a = fade;
                windowCover.color = spriteColor;
                yield return null;
            }
            fade = 1f;
            spriteColor.a = fade;
            windowCover.color = spriteColor;

            //fade out
            startTime = Time.time;
            while (fade > 0f)
            {
                fade = Mathf.Lerp(1f, 0f, (Time.time - startTime) / (effectTimer - 0.1f));
                spriteColor.a = fade;
                windowCover.color = spriteColor;
                yield return null;
            }
            fade = 0f;
            spriteColor.a = fade;
            windowCover.color = spriteColor;

            onEffectFinish();
            yield break;
        }
    }
    //same as above but leaves text box
    IEnumerator flash_no_window()
    {
        float fade = 0f;
        float startTime;
        Color spriteColor = new Color(1f, 1f, 1f, 0f);  //white
        noWindowCover.color = spriteColor;
        while (true)
        {
            //sudden flash
            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / 0.1f);
                spriteColor.a = fade;
                noWindowCover.color = spriteColor;
                yield return null;
            }
            fade = 1f;
            spriteColor.a = fade;
            noWindowCover.color = spriteColor;

            //fade out
            startTime = Time.time;
            while (fade > 0f)
            {
                fade = Mathf.Lerp(1f, 0f, (Time.time - startTime) / (effectTimer - 0.1f));
                spriteColor.a = fade;
                noWindowCover.color = spriteColor;
                yield return null;
            }
            fade = 0f;
            spriteColor.a = fade;
            noWindowCover.color = spriteColor;

            onEffectFinish();
            yield break;
        }
    }

    //the black screen from fade has to go away right..., waits for indicated amount of time before removing screen
    IEnumerator unfade_window()
    {
        if (windowCover.color.a > 0f)
        {
            yield return new WaitForSeconds(effectTimer);
            windowCover.color = new Color(0f, 0f, 0f, 0f);
        }
        onEffectFinish();
        yield return null;
    }

    IEnumerator unfade_no_window()
    {
        if (noWindowCover.color.a > 0f)
        {
            yield return new WaitForSeconds(effectTimer);
            noWindowCover.color = new Color(0f, 0f, 0f, 0f);
        }
        onEffectFinish();
        yield return null;
    }
}
