using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[System.Serializable]
public class Boundary
{
    public float xmin, xmax, zmin, zmax;
    public Boundary(float xmin, float xmax, float zmin, float zmax)
    {
        this.xmin = xmin;
        this.xmax = xmax;
        this.zmin = zmin;
        this.zmax = zmax;
    }
}

//remake now doesnt let player shoot normally, player picks an option to shoot anytime during a round
//if correct, then continue to next round, if lsat round, then done, if incorrect, lose hp?
//normally the enemy will shoot bullets out based on predetermined pattern, and getting hit will lose hp and gain Iframes
//
public class BHPlayerController : MonoBehaviour {

    public float speed = 8.0f;
    float moveHoriz, moveVert;
    public Vector3 initialPos;
    Rigidbody rb;

    public static Boundary bound = new Boundary(-4.5f, 4.5f, 0f, 7f); // bounds of the game

    public GameObject enemy;
    public GameObject playerBullet;
    //private float fireRate = 3.0f;//per second
    //private bool canshoot = true;
    public bool playerControl = true;
    public bool inOption;
    public int hoverOption;

    public int hp;

    void Start()
    {
        inOption = false;
        rb = GetComponent<Rigidbody>();
        hp = 10;
        //bound.xmin = -4;
        //bound.xmax = 4;
        //bound.zmin = -4.5f;
        //bound.zmax = 4.5f;
    }

    void Update()
    {
        //player controls should be changed to be smoother instead of using input.getaxis, but its fine for now
        if (playerControl){
            //if (Input.GetKey(KeyCode.C) && canshoot)
            //{
            //    shoot();
            //}
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed = 3.0f;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed = 8.0f;
            }
            if(Input.GetKey(KeyCode.A) && inOption)
            {
                LogicFrenzyController.instance.onSelect(hoverOption);
            }
            moveHoriz = Input.GetAxis("Horizontal");
            moveVert = Input.GetAxis("Vertical");
            rb.position = rb.position + new Vector3(moveHoriz * speed * Time.deltaTime, 1.0f, moveVert * speed * Time.deltaTime);
            rb.position = new Vector3(
                Mathf.Clamp(rb.position.x, bound.xmin, bound.xmax),
                1.0f,
                Mathf.Clamp(rb.position.z, bound.zmin, bound.zmax)
                );
        }
    }

    //the logic shouldbe done in the controller, the player will always shoot the same thing, probably
/*    void shoot()
    {
        //needs to be replaced with selecting response to shoot
        GameObject bullet = Instantiate(playerBullet) as GameObject;
        bullet.transform.position = gameObject.transform.position;
        bullet.SetActive(true);
        StartCoroutine("reload");
    }
*/
/*   IEnumerator reload()
    {
        canshoot = false;
        yield return new WaitForSeconds(1/fireRate);
        canshoot = true;
    }*/

    bool invincible;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "playerShot" && !invincible && playerControl)
        {
            Bullet tmp = other.gameObject.GetComponent<Bullet>();
            hp -= 1;
            LogicFrenzyController.instance.loseHP();
            if(hp == 0)
            {
                LogicFrenzyController.instance.zeroHP();
            }
            StartCoroutine("IFrames");
        }
        else if(other.tag == "playerShot")
        {
            //an option was chosen by running into it
            //report the option to logic frenzy controller
            //just set the name of the object as the index of its corresponding choice
            Debug.Log("hit option");
            inOption = true;
            hoverOption = int.Parse(other.gameObject.name);
            LogicFrenzyController.instance.setOptionText(hoverOption, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "playerShot")
        {
            inOption = false;
            LogicFrenzyController.instance.setOptionText(hoverOption, false);
        }
    }

    IEnumerator IFrames()
    {
        invincible = true;
        yield return new WaitForSeconds(2.0f);
        invincible = false;
    }

    public void setPlayerControl(bool state)   //enable/disable player controls
    {
        playerControl = state;
    }

    public void center()    //centers the player on the screen
    {
        StartCoroutine(returntocenter());
    }

    IEnumerator returntocenter()
    {
        while (transform.position != initialPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPos, 0.1f);
            yield return new WaitForSeconds(0.01f);
        }
        //Debug.Log(initialPos);
        //transform.localPosition = initialPos;
        //yield return null;
    }
}
