using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeTimeInteractableObjectUI : MonoBehaviour {

    public InteractableObject obj;

    public GameObject hovermenu;
    public GameObject examineButton;
    public GameObject talkButton;
    public Image image;

    public void init(InteractableObject obj)
    {
        this.obj = obj;
        if (obj.isChar)
        {
            examineButton.SetActive(false);
        }
        else
        {
            talkButton.SetActive(false);
        }
        image.sprite = SpriteAtlas.instance.loadSprite(obj.sprite);
        //set up anchors for object in the scene
        RectTransform currObj = gameObject.GetComponent<RectTransform>();
        currObj.transform.localPosition = new Vector3(0, 0, 0);
        currObj.anchorMin = new Vector2(obj.xmin, obj.ymin);
        currObj.anchorMax = new Vector2(obj.xmax, obj.ymax);
    }

    //for characters
    public void OnTalk()
    {
        FreeTimeUI.instance.confirmDesc.text = "Do you want to spend time with " + "<color=lime>" + NameAtlas.instance.getName(obj.name) + "</color>" + " ?";
        FreeTimeUI.instance.confirmMenu.SetActive(true);
        FreeTimeUI.instance.talkConfirm += OnConfirm;
    }

    public void OnConfirm()
    {
        string name = NameAtlas.instance.getName(obj.name);
        FreeTimeUI.instance.onInteract(obj.freetimeConvos[storyProgress.instance.flags[name]], obj.isChar);
        OnCancel();
    }

    public void OnCancel()
    {
        hovermenu.SetActive(false);
        FreeTimeUI.instance.confirmMenu.SetActive(false);
        FreeTimeUI.instance.talkConfirm -= OnConfirm;
    }

    //for objects
    public void OnExamine()
    {
        FreeTimeUI.instance.onInteract(obj.examineFile, obj.isChar);
    }

    public void HoverOn()
    {
        hovermenu.SetActive(true);
    }

    public void HoverOff()
    {
        hovermenu.SetActive(false);
    }
}
