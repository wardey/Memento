using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ConvoButton : MonoBehaviour{

    public Action<string> clickAction;
    public string topic;
    public Text display;

	public void onClick()
    {
        Debug.Log("doing something with click " + topic);
        clickAction(topic);
    }

    public void setTopic(string topic)
    {
        Debug.Log("changing topic " + topic);
        this.topic = topic;
        display.text = topic;
    }
}
