using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InteractableObjectUI : MonoBehaviour {

    public InteractableObject obj;

    //ui purposes only
    public GameObject hovermenu;    //needs to stay open if talk menu is open
    public GameObject talkmenu;
    public List<ConvoButton> convoButtons;   //only 5, enable and disable based on number of convos to be displayed in talk menu

    //for modifying cursor
    public Texture2D mikuCursor;
    public CursorMode cursorMode = CursorMode.ForceSoftware;
    public Vector2 hotSpot = Vector2.zero;

    public GameObject talkButton;
    public GameObject examineButton;
    public GameObject presentButton;

    public Image image;

    public void init(InteractableObject obj)
    {
        this.obj = obj;
        for (int i = 0; i < convoButtons.Count; i++)
        {
            convoButtons[i].clickAction += OnConvo;
        }
        if (obj.isChar)
        {
            examineButton.SetActive(false);
        }
        else
        {
            talkButton.SetActive(false);
            presentButton.SetActive(false);
        }
        image.sprite = SpriteAtlas.instance.loadSprite(obj.sprite);
        //set up anchors for object in the scene
        RectTransform currObj = gameObject.GetComponent<RectTransform>();
        currObj.transform.localPosition = new Vector3(0, 0, 0);
        currObj.anchorMin = new Vector2(obj.xmin, obj.ymin);
        currObj.anchorMax = new Vector2(obj.xmax, obj.ymax);
    }

    public void OnTalk()
    {
        //open convo menu and modify convo buttons accordingly
        int counter = 0;
        for (int i = 0; i < obj.convos.Count; i++)
        {
            if (obj.convos[i].requirementType == "none" || obj.convos[i].complete)   //no requirement, or already indicated as complete
            {
                convoButtons[counter].setTopic(obj.convos[i].displayName);
                convoButtons[counter].gameObject.SetActive(true);
                obj.convos[i].display = true;
                counter++;
            }
            else if (obj.convos[i].requirementType == "affection")  //based on affection
            {
                //if the affection stored in flags for this character matches the int equivalent of the requirement
                if (storyProgress.instance.flags[NameAtlas.instance.getName(name)] == int.Parse(obj.convos[i].requirement))
                {
                    convoButtons[counter].setTopic(obj.convos[i].displayName);
                    convoButtons[counter].gameObject.SetActive(true);
                    obj.convos[i].display = true;
                    counter++;
                }
            }
            else if (obj.convos[i].requirementType == "own") //based on evidence possession
            {
                int length = GameController.evidenceBag.Count;
                for (int j = 0; j < length; j++)
                {
                    //if the required evidence is in the evidence bag
                    if (GameController.evidenceBag[j].name == obj.convos[i].requirement)
                    {
                        if(obj.convos[i].action == "update")
                        {
                            //find the convo with the same display name as the current one and has display = true, copy current one into that spot, remove this one, and i--
                            if (obj.convos[i].display)
                            {
                                convoButtons[counter].setTopic(obj.convos[i].displayName);
                                convoButtons[counter].gameObject.SetActive(true);
                                counter++;
                                break;
                            }
                            else     //a convo that is not being displayed, so it is one part of the convo chain for this topic
                            {
                                //check that the predecessor convo is complete, default starting topic is 0
                                for(int k = 0; k < i; k++)
                                {
                                    if(obj.convos[k].displayName == obj.convos[i].displayName && obj.convos[k].progress == obj.convos[i].progress - 1 && obj.convos[k].complete)//found complete predecessor
                                    {
                                        //replace predecessor in the list
                                        obj.convos[k] = obj.convos[i];
                                        obj.convos.RemoveAt(i);
                                        i--;
                                        //find and update the topic button with the right convo
                                        for(int l = 0; l < convoButtons.Count; l++)
                                        {
                                            if(convoButtons[l].topic == obj.convos[k].displayName)
                                            {
                                                convoButtons[l].setTopic(obj.convos[k].displayName);
                                                convoButtons[l].gameObject.SetActive(true);
                                                obj.convos[k].display = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            convoButtons[counter].setTopic(obj.convos[i].displayName);
                            convoButtons[counter].gameObject.SetActive(true);
                            obj.convos[i].display = true;
                            counter++;
                            break;
                        }
                    }
                }
            }
            else if (obj.convos[i].requirementType == "present")    //based on whether certain evidence has been presented
            {
                //if the evidence response for this specific evidence is complete, then the requirement is unlocked
                for (int j = 0; j < obj.evidenceResponse.Count; j++)
                {
                    if (obj.evidenceResponse[j].evidence == obj.convos[i].requirement && obj.evidenceResponse[j].complete)  //requirements for convo are met
                    {
                        //this means that there is currently a topic with the same name as the current convo, need to find it and replace it
                        if (obj.convos[i].action == "update")
                        {
                            //find the convo with the same display name as the current one and has display = true, copy current one into that spot, remove this one, and i--
                            if (obj.convos[i].display)
                            {
                                counter++;   //already being displayed, so just keep going
                                break;
                            }
                            else     //a convo that is not being displayed, so it is one part of the convo chain for this topic
                            {
                                for (int m = 0; m < obj.convos.Count; m++)
                                {
                                    Debug.Log(obj.convos[m].displayName);
                                }
                                //check that the predecessor convo is complete, default starting topic is 0
                                for (int k = 0; k < i; k++)
                                {
                                    if (obj.convos[k].displayName == obj.convos[i].displayName && obj.convos[k].progress == obj.convos[i].progress - 1 && obj.convos[k].complete)//found complete predecessor
                                    {
                                        //replace predecessor in the list
                                        obj.convos[k] = obj.convos[i];
                                        obj.convos.RemoveAt(i);
                                        i--;
                                        //find and update the topic button with the right convo
                                        for (int l = 0; l < convoButtons.Count; l++)
                                        {
                                            if (convoButtons[l].topic == obj.convos[k].displayName)
                                            {
                                                convoButtons[l].setTopic(obj.convos[k].displayName);
                                                convoButtons[l].gameObject.SetActive(true);
                                                obj.convos[k].display = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            convoButtons[counter].setTopic(obj.convos[i].displayName);
                            convoButtons[counter].gameObject.SetActive(true);
                            obj.convos[i].display = true;
                            counter++;
                            break;
                        }
                    }
                }
            }
            else
            {
                obj.convos[i].display = false;
            }
        }
        //disable all other buttons
        for (int i = counter; i < convoButtons.Count; i++)
        {
            convoButtons[i].gameObject.SetActive(false);
        }
        //enable menu game object
        talkmenu.gameObject.SetActive(true);
    }

    public void OnEvidence() //for opening evidence menu
    {
        //open evidence menu
        InvestigationUI.instance.evidenceMenu.gameObject.SetActive(true);
        InvestigationUI.instance.evidenceMenu.PresentButton.SetActive(true);
        //attach the onPresent method from the current character to the evidence menu's present action
        InvestigationUI.instance.evidenceMenu.PresentCheck += OnPresent;
    }

    public void OnPresent(Evidence evidence) //should be able to attach to evidence menu script to get the evidence being presented
    {
        //calls investigationui's oninteraction(string) with the filename to play
        for (int i = 0; i < obj.evidenceResponse.Count; i++)
        {
            if (obj.evidenceResponse[i].evidence == evidence.name)
            {
                obj.evidenceResponse[i].complete = true;
                InvestigationUI.instance.onInteract(obj.evidenceResponse[i].filename);
                checkEvidenceComplete();
                return;
            }
        }
        //did not find suitable evidence, give default file
        InvestigationUI.instance.onInteract(obj.badEvidenceFile);
    }

    void checkEvidenceComplete()
    {
        for (int i = 0; i < obj.evidenceResponse.Count; i++)
        {
            if (!obj.evidenceResponse[i].complete) return;
        }
        obj.evidenceComplete = true;
        if (obj.convoComplete) obj.complete = true;
    }

    //max 5 buttons
    public void OnConvo(string topic)   //make interaction buttons and attach scripts to them that return the name of the button(topic)
    {
        //calls investigationui's oninteraction(string) with the filename to play
        for (int i = 0; i < obj.convos.Count; i++)
        {
            if (obj.convos[i].displayName == topic)
            {
                obj.convos[i].complete = true;
                if (obj.convos[i].filename == InvestigationUI.instance.currData.shorcutFile) InvestigationUI.instance.shortCircuit = true;
                InvestigationUI.instance.onInteract(obj.convos[i].filename);
                checkConvoComplete();
                break;
            }
        }
    }

    void checkConvoComplete()
    {
        for (int i = 0; i < obj.convos.Count; i++)
        {
            if (obj.convos[i].requirementType != "affection" && !obj.convos[i].complete) return;
        }
        obj.convoComplete = true;
        if (obj.evidenceComplete) obj.complete = true;
    }

    //only for objects, only 1 file to play
    public void OnExamine()
    {
        if (obj.examineFile == InvestigationUI.instance.currData.shorcutFile) InvestigationUI.instance.shortCircuit = true;
        InvestigationUI.instance.onInteract(obj.examineFile);
        obj.complete = true;
    }

    //add on mouse over, display a clickable menu that shows either talk/present or examine based on isChar
    //should be a circular menu that surrounds object, probably, and has fade transition
    //also menu that appears should detect hover and stay on screen even if mouse moves off of original object

    public void HoverOn()
    {
        //cursorSet(mikuCursor);
        hovermenu.SetActive(true);
    }

    public void HoverOff()
    {
        //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        hovermenu.SetActive(false);
        talkmenu.SetActive(false);
    }

    void cursorSet(Texture2D tex)
    {
        CursorMode mode = CursorMode.ForceSoftware;
        float xspot = tex.width / 2;
        float yspot = tex.height / 2;
        Vector2 hotSpot = new Vector2(xspot, yspot);
        Cursor.SetCursor(tex, hotSpot, mode);
    }
}
