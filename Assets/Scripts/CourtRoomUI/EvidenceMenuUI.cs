using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class EvidenceMenuUI : MonoBehaviour
{
    public Action<Evidence> PresentCheck;
    private List<Evidence> evidenceList;   //dont need, can just use courtroomui's evidencebag
    private Statement currStatement;
    private int currEvidenceIndex;
    private bool prompt;
    private string promptEvidence;

    public List<Text> display = new List<Text>();
    public Transform displayHighlight;
    public Text currStatementText;
    public Text evidenceDesc;
    public Image evidenceImage;

    public GameObject PresentButton;

    void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            if(currEvidenceIndex < evidenceList.Count - 1)
            {
                currEvidenceIndex++;
                repaint();
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if (currEvidenceIndex > 0)
            {
                currEvidenceIndex--;
                repaint();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currEvidenceIndex < evidenceList.Count - 1)
            {
                currEvidenceIndex++;
                repaint();
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currEvidenceIndex > 0)
            {
                currEvidenceIndex--;
                repaint();
            }
        }
        //need to add in corresponding animations for these buttons
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            onPresent();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            onBack();
        }
    }

    void OnEnable()
    {
        Debug.Log(GameController.evidenceBag.Count);
        transform.Find("Present").GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
        for (int i = 0; i < GameController.evidenceBag.Count; i++)
        {
            Debug.Log(GameController.evidenceBag[i].name);
        }
        //assumes current statement is set, but will only be set in debate mode, ignored otherwise
        evidenceList = GameController.evidenceBag;
        currEvidenceIndex = 0;
        repaint();
    }

    void repaint()
    {
        //always keep evidenceList[currEvidenceIndex] at display[3]
        for(int i = 0; i < 5; i++)
        {
            display[i].text = "";
        }
        display[2].text = evidenceList[currEvidenceIndex].name;
        evidenceImage.sprite = SpriteAtlas.instance.loadSprite(evidenceList[currEvidenceIndex].image);
        evidenceDesc.text = evidenceList[currEvidenceIndex].desc;
        for (int i = 1; i <= 2; i++) //3 spaces before current evidence
        {
            if(currEvidenceIndex - i >= 0)
            {
                display[2 - i].text = evidenceList[currEvidenceIndex - i].name;
            }
        }
        for (int i = 1; i <= 2; i++) //3 spaces after current evidence
        {
            if (currEvidenceIndex + i < evidenceList.Count)
            {
                display[i + 2].text = evidenceList[currEvidenceIndex + i].name;
            }
        }
    }

    public void setCurrStatement(Statement statement)
    {
        currStatement = statement;
        currStatementText.text = currStatement.statement;
    }

    public void onBack()
    {
        //CourtRoomUI.instance.EvidenceButton.SetActive(true);
        //gameObject.SetActive(false);
        StartCoroutine(delayBack());
    }

    IEnumerator delayBack()
    {
        yield return new WaitForSeconds(0.15f);
        CourtRoomUI.instance.EvidenceButton.SetActive(true);
        gameObject.SetActive(false);
    }

    public void onPresent()
    {
        if (prompt)
        {
            GameObject.Find("Game").GetComponent<GameController>().skipUntilPresent = evidenceList[currEvidenceIndex].name == promptEvidence;
            prompt = false;
            if (transform.parent.GetComponent<DebateDialogueUI>() != null)
            {
                DebateDialogueUI.instance.onClickDialogue();
            }
            else if (transform.parent.GetComponent<DialogueUI>() != null)
            {
                DialogueUI.instance.onClickDialogue();
            }
        }
        else
            PresentCheck(evidenceList[currEvidenceIndex]);
        gameObject.SetActive(false);
    }

    public void presentprompt(string statement, string evidence)    //does this need to lose hp?
    {
        prompt = true;
        //set the current statement
        currStatementText.text = statement;
        //set the corresponding evidence
        promptEvidence = evidence;
    }
}
