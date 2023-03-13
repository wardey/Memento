using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private float walkSpd = 3.0f;
    private float rootTwo = Mathf.Sqrt(2);

    private bool up, down, left, right;

    public List<Image> objs;
	// Use this for initialization
	void Start () {
        //walkSpd =  Screen.width / 20;
        up = down = left = right = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))    //forward right
        {
            if (up)
            {
                transform.position = transform.position + new Vector3(0, Time.deltaTime * walkSpd / rootTwo, 0);
                down = true;
            }
            if (right)
            {
                transform.position = transform.position + new Vector3(Time.deltaTime * walkSpd / rootTwo, 0, 0);
                left = true;
            }
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))    //forward left
        {
            if (up)
            {
                transform.position = transform.position + new Vector3(0, Time.deltaTime * walkSpd / rootTwo, 0);
                down = true;
            }
            if (left)
            {
                transform.position = transform.position - new Vector3(Time.deltaTime * walkSpd / rootTwo, 0, 0);
                right = true;
            }
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))    //backward right
        {
            if (down)
            {
                transform.position = transform.position - new Vector3(0, Time.deltaTime * walkSpd / rootTwo, 0);
                up = true;
            }
            if (right)
            {
                transform.position = transform.position + new Vector3(Time.deltaTime * walkSpd / rootTwo, 0, 0);
                left = true;
            }
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))    //backward left
        {
            if (down)
            {
                transform.position = transform.position - new Vector3(0, Time.deltaTime * walkSpd / rootTwo, 0);
                up = true;
            }
            if (left)
            {
                transform.position = transform.position - new Vector3(Time.deltaTime * walkSpd / rootTwo, 0, 0);
                right = true;
            }
        }
        else if (Input.GetKey(KeyCode.W))    //forward
        {
            if (up)
            {
                transform.position = transform.position + new Vector3(0, Time.deltaTime * walkSpd, 0);
                down = true;
            }
        }
        else if (Input.GetKey(KeyCode.S))   //backward
        {
            if (down)
            {
                transform.position = transform.position - new Vector3(0, Time.deltaTime * walkSpd, 0);
                up = true;
            }
        }
        else if (Input.GetKey(KeyCode.A))   //left
        {
            if (left)
            {
                transform.position = transform.position - new Vector3(Time.deltaTime * walkSpd, 0, 0);
                right = true;
            }
        }
        else if (Input.GetKey(KeyCode.D))   //right
        {
            if (right)
            {
                transform.position = transform.position + new Vector3(Time.deltaTime * walkSpd, 0, 0);
                left = true;
            }
        }
        //fixLayering();
    }

    void fixLayering()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("hit wall");
        if (other.tag == "right") //wall to the right
        {
            Debug.Log("right false");
            right = false;
        }
        else if (other.tag == "left") //wall to the left
        {
            Debug.Log("left false");
            left = false;
        }
        else if (other.tag == "up") //wall up
        {
            Debug.Log("up false");
            up = false;
        }
        else if (other.tag == "down") //wall down
        {
            Debug.Log("down false");
            down = false;
        }
        else
        {
            Debug.Log("hit unclassified object");
        }
    }
}