using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {

    public static DialogueUI instance;
    public Action onEffectFinish;
    public Action onClickDialogue;

    //text box display stuffs
    public GameObject arrow;
    public Text dialogue;
    private float letterDelay = 0.01f;
    private string str;
    private bool dialogueDisplayDone = false;
    private bool skipCurrDialogue = false;
    IEnumerator textDisplay;
    Color32 skyBlue = new Color32(133, 206, 250, 255);

    //character display
    //the spot at chars[1] is shared by middle and center, if one of them is in use, then char[1] != ""
    private List<string> chars = new List<string>(3);    //list containing the chars(name of sprite files) on screen, from left to right, "" indicates no character in that spot
    public characterImage characterLeft;
    public characterImage characterRight;
    public characterImage characterMiddle;
    public characterImage characterCenter;  //for only 1 character on screen
    public Text speaker;

    public Image item;

    // Use this for initialization
    void Awake() {
        instance = this;
        chars = new List<string>{"","",""};
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    characterImage findChar(string name)    //returns the characterImage that correspondes to the name
    {
        if (characterLeft.name == name)
        {
            return characterLeft;
        }
        else if (characterRight.name == name)
        {
            return characterRight;
        }
        else if(characterMiddle.name == name)
        {
            return characterMiddle;
        }
        else if(characterCenter.name == name)
        {
            return characterCenter;
        }
        return characterMiddle; //assumes that a valid name will always be given
    }

    public void OnClickDialogue()
    {
        if (dialogueDisplayDone == false)
        {
            skipCurrDialogue = true;
            //StopCoroutine(textDisplay);
        }
        else
        {
            //update content from dialogueparser class
            //displayLine();
            //tell game controller that the player clicked, and to go to the next line
            onClickDialogue();
        }
    }

    //need to create new effects for all of these...methods, refer to doc
    public List<string> setChar(string name, string pos)    //force set a character to a certain position
    {
        //return the current positions of characters back to controller
        if(pos == "l")
        {
            characterLeft.setImage(name);   //maybe play effect here? should be done on characterImage side
            chars[0] = name;
        }
        else if(pos == "m")
        {
            characterMiddle.setImage(name); //set as middle for now, if only 1 char on screen, adjust will change it to center
            chars[1] = name;
        }
        else if(pos == "r")
        {
            characterRight.setImage(name);
            chars[2] = name;
        }
        adjust();   //adjusts for middle/center discrepancy, and positional changes
        return chars;
    }

    public List<string> addChar(string name)    //adds a character to the dialogue ui, and shifts other characters around if need be
    {
        if ((chars[0] == "" && chars[1] == "" && chars[2] == "") || (chars[0] != "" && chars[1] == "" && chars[2] != ""))
        //no chars on screen, defaults middle or two characters at left and right
        {
            characterMiddle.setImage(name);
            chars[1] = name;
        }
        //not possible to have one character on the side and one in the middle, it would shift
        else if (chars[2] == "")   //there is only one character, and its not the one on the right, so set new one to right and adjust
        {
            characterRight.setImage(name);
            chars[2] = name;
        }
        else //only one character, on the right, set new to left
        {
            characterLeft.setImage(name);
            chars[0] = name;
        }
        //what if there are 3 characters on the screen? shouldnt happen.
        adjust();
        return chars;
    }

    public void changeSpeaker(string name)  //only change speaker text
    {
		speaker.text = NameAtlas.ToFirstName(NameAtlas.instance.getName(name)).ToUpper();
    }

    public void changeSpeaker(string name, string emote)    //if need to change speaker and sprite
    {
		speaker.text = NameAtlas.ToFirstName(NameAtlas.instance.getName(name)).ToUpper();
        findChar(name).setImage(name + "_" + emote);    //assuming naming convention of sprite files is charactername_emote#
        storyProgress.instance.mostRecentSprite[NameAtlas.instance.getName(name)] = name + "_" + emote;
    }

    public void removeChar(string name) //removes a character from the screen and adjusts the others to their new positions
    {
        for(int i = 0; i<3; i++)
        {
            if(chars[i].Split('_')[0] == name)
            {
                chars[i] = "";
                findChar(name).remove();
                break;
            }
        }
        adjust();
    }

    public void clearAll()
    {
        for (int i = 0; i < 3; i++)
        {
            if(chars[i] != "")
            {
                removeChar(chars[i].Split('_')[0]);
            }
        }
        str = "";
        dialogue.text = str;
        dialogueDisplayDone = false;
        skipCurrDialogue = false;
        speaker.text = "";
    }

    public void charEffect(string fnName, string pos)//play a character effect on the character at position, or all if pos is a
    {
        //havent made them yet
    }

    public void adjust()    //adjusts the character positions, also sets the elements in chars to match the names of the characters at each position
    {
        //looks at the position of the characters and shifts them to according to the rules
        //these are positions after the new char is added
        if (chars[0] == "" && chars[1] != "" && chars[2] == "" && characterMiddle.name != "")
        {   //only one character on screen and at middle, change it to center
            characterCenter.setImage(characterMiddle.image);
            characterMiddle.remove();
        }
        else if (chars[0] != "" && chars[1] != "" && chars[2] == "")  //more than 1 character on screen, adjust if one is at the center
        {
            //characters at left and middle/center, shift the one in the middle to right
            if(characterMiddle.name != "")
            {
                characterRight.setImage(characterMiddle.image);
                characterMiddle.remove();
            }
            else
            {
                characterRight.setImage(characterCenter.image);
                characterCenter.remove();
            }
            chars[1] = "";
            chars[2] = characterRight.image;
        }
        //characters at middle/center and right, shift middle to left
        else if(chars[0] == "" && chars[1] != "" && chars[2] != "")
        {
            if (characterMiddle.name != "")
            {
                characterLeft.setImage(characterMiddle.image);
                characterMiddle.remove();
            }
            else
            {
                characterLeft.setImage(characterCenter.image);
                characterCenter.remove();
            }
            chars[1] = "";
            chars[0] = characterLeft.image;
        }
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
        while(counter < line.Length)
        {
            if(line[counter] == '<')
            {
                counter++;
                newLine += "<color=";
                while(line[counter] != ':')
                {
                    newLine += line[counter];
                    counter++;
                }
                newLine += ">";
                counter++;  //skips the :
                while(line[counter] != '>')
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
        int i = 0,j;
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
            if(strComplete[i] == '<') //start of markup
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
                while(strComplete[i] != '<')    //skip text
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
                while(strComplete[j] != '<')
                {
                    str = str.Insert(j, strComplete[j].ToString());
                    j++;
                    dialogue.text = str;
                    yield return new WaitForSeconds(letterDelay);
                }
            }
            if(i < strComplete.Length) str += strComplete[i++];
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
