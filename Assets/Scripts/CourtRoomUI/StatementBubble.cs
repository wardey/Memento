using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatementBubble : MonoBehaviour {

    public Action<Statement> onClick;
    public Statement statement;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick()
    {
        onClick(statement);
    }
}
