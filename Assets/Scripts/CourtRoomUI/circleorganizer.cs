using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// should split the transitions into three parts, and call each part after the other. when each part is done, call back another method to call the next part
/// since all are coroutines, mix and mashing different shots, transitions, and effects need to wait until the previous part is done
/// Always transition->shot->effects
/// </summary>
public class circleorganizer : MonoBehaviour {

    /// <summary>
    /// To Do list:
    /// -round circle spin, not just spin on the spot, rotate around inner circle while turning camera to maintain same angle (done)
    /// -fast zoom in and out (done)
    /// -set starting position for panning, using side/vertical tilt for doing this, but need to make it smooth, combine it with turning to turning to the starting position
    /// </summary>
    /// 
    //need to set up the circle with the characters, so need to know the first focus character, and transition to that char when button is press/or a click happens
    //for debate section, pressing next will get the next focus character/statement, repopulate the circle, and get the transition to that character, then execute it
    //for vn section, clicking will do the same as above, but just reads the next line of the txt file for the dialogue line
    private List<Transform> characters = new List<Transform>();
    public float ratio; //needs to be set based on the screen resolution(mostly width) to change the distance of transitions
    public int distance;
    public GameObject CameraAnchor; //the point at the center of the circle, camera is always on a line extending out from this point's forward, looking the same direction
    public Camera debateCamera;
    private bool smoothTurn;

    //since we can only do string arguments, put all arguments as fields
    public Action onTransitionComplete;
    public int currChar;
    private Vector3 turnoffset;

    // Use this for initialization
    void Start() {
        organizeCircle(15);
        //StartCoroutine(fullTransition(3));
    }

    // Update is called once per frame
    void Update()
    {

    }

    //24 degrees between each char
    void organizeCircle(int numChars)
    {
        for (int i = 0; i < numChars; i++)
        {
            Transform tmp = transform.GetChild(i);
            characters.Add(tmp);
            tmp.localPosition = new Vector3(distance * Mathf.Sin(i * 2 * Mathf.PI / 15), 0, distance * Mathf.Cos(i * 2 * Mathf.PI / 15));
        }
    }

    public void repopulateWithRecentSprites(List<string> chars)
    {
        //make a static global list of characters with their most recent sprite(emote number)
        for (int i = 0; i < 15; i++)
        {
            Sprite sprite = SpriteAtlas.instance.loadSprite(storyProgress.instance.mostRecentSprite[chars[i]]);
            characters[i].Find("Front").GetComponent<SpriteRenderer>().sprite = sprite;
            characters[i].Find("Back").GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    public void repopulateWithSprites(List<string> sprites)
    {
        //make a static global list of characters with their most recent sprite(emote number)
        for (int i = 0; i < 15; i++)
        {
            Sprite sprite = SpriteAtlas.instance.loadSprite(sprites[i]);
            characters[i].Find("Front").GetComponent<SpriteRenderer>().sprite = sprite;
            characters[i].Find("Back").GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    //need to make regular debate call this with a premade list of the sprites, can make it when repopulating in debate
    public void repopulateWithSprites(List<string> chars, List<string> sprites)
    {
        //make a static global list of characters with their most recent sprite(emote number)
        for (int i = 0; i < 15; i++)
        {
            //Debug.Log(i + " " + sprites[i]);
            characters[i].Find("Front").GetComponent<SpriteRenderer>().sprite = SpriteAtlas.instance.loadSprite(sprites[i]);
            characters[i].Find("Back").GetComponent<SpriteRenderer>().sprite = SpriteAtlas.instance.loadSprite(sprites[i]);
        }
    }

    public IEnumerator transitionToCurrChar(Transition transition)
    {
        StopAllCoroutines();
        Debug.Log("starting a transition");
        resetCamera();
        if (transition.preset != "")
        {
            Debug.Log(transition.preset);
            yield return StartCoroutine(transition.preset);
        }
        else
        {
            Debug.Log(transition.transition);
            Debug.Log(transition.shot);
            Debug.Log(transition.movement);
            setOffset(transition.movement);
            if (transition.shot.Contains("_i"))
            {
                yield return StartCoroutine(transition.shot);
                yield return StartCoroutine(transition.transition);
            }
            else
            {
                yield return StartCoroutine(transition.transition);
                yield return StartCoroutine(transition.shot);
            }
            yield return StartCoroutine(transition.movement);
        }
        onTransitionComplete();
    }

    //could try doing a for loop that iterates through coroutines, each representing a part of the transition
    IEnumerator fullTransition(int charNumber)
    {
        Debug.Log("first set");
        resetCamera();  //should be at the start of every transition
        //tilt(5);
        yield return zoomIn(true, 2);
        yield return smoothLook(shotOffset(characters[3], 0, -0.5f, 0), false);
        //verticalTilt(-3f);
        yield return pan("up", 0.7f, 2);

        Debug.Log("second set");
        resetCamera();
        cameraTilt(3);
        yield return smoothLook(shotOffset(characters[0], -1, 0, 0), true);
        yield return zoomIn(false, 3);
        //sideTilt(-3f);
        yield return pan("right", 1.5f, 2);
        yield return new WaitForSeconds(0.3f);

        yield return zoomOut(false, 3);
        yield return pan("left", 0.5f, 2);

        Debug.Log("third set");
        resetCamera();
        cameraTilt(3);
        yield return zoomIn(true, 2);
        yield return smoothLook(shotOffset(characters[9], 1.5f, 0.3f, 0), false);
        StartCoroutine(posPan("left", 0.8f, 2));
        StartCoroutine(pan("left", 1f, 2));
        yield return null;
    }

    /********************Create presets here, the methods are basically done below*********************/

    //since the turning needs to know the position beforehand, that means I need to know the panning direction first, and find the offset to add onto the position
    //that its going to turn to. Even though the execution order is 
    //if(zoom) instant turn, then zoom
    //else zoom, turn
    //then pan
    //turn needs to look ahead at pan, and set a variable 

    IEnumerator defaultTransition()
    {
        yield return smoothLook(shotOffset(characters[currChar], 0, 0, 0), true);
        StartCoroutine(perpetualMotion("right"));
    }
    IEnumerator defaultTransition2()
    {
        yield return smoothLook(shotOffset(characters[currChar], 0, 0, 0), true);
    }

    IEnumerator dedeucePreset()
    {
        yield return StartCoroutine(smoothLook(characters[currChar].position, false));
    }

    IEnumerator testpreset()
    {
        yield return zoomIn(true, 2);
        yield return smoothLook(shotOffset(characters[currChar], -1f, 0, 0), false);
        yield return moveRight();
    }

    IEnumerator preset1()
    {
        yield return Turn();
        yield return normal_i();
        yield return moveLeft();
    }

    IEnumerator preset2()
    {
        yield return Turn_i();
        yield return close_i();
        yield return moveUp();
    }

    //this one looks weird
    IEnumerator focus_zoomout()
    {
        yield return StartCoroutine(zoomIn(true, 2));
        yield return smoothLook(shotOffset(characters[currChar], 0, 0, 0), false);
        StartCoroutine(twirl_right());
        yield return StartCoroutine(zoomOut(false, 3));
    }

    //this one doesnt work as intended
    IEnumerator zoomin_twirl()
    {
        yield return StartCoroutine(far_i());
        yield return smoothLook(shotOffset(characters[currChar], 0, 0, 0), true);
        StartCoroutine(twirl_right());
        yield return StartCoroutine(zoomIn(false, 4));
    }

    IEnumerator zoomout_left()
    {
        StartCoroutine(far());
        yield return smoothLook(shotOffset(characters[currChar], 1, 0, 0), true);
        yield return StartCoroutine(moveLeft());
    }

    IEnumerator zoomout_right()
    {
        StartCoroutine(far());
        yield return smoothLook(shotOffset(characters[currChar], -1, 0, 0), true);
        yield return StartCoroutine(moveRight());
    }

    IEnumerator diagonal_moveup()
    {
        yield return StartCoroutine(normal_i());
        sideTilt(-10);
        yield return StartCoroutine(smoothLook(shotOffset(characters[currChar], -0.5f, -1f, 0), true));
        yield return StartCoroutine(moveUp());
    }

    IEnumerator diagonal_movedown()
    {
        yield return StartCoroutine(normal_i());
        sideTilt(10);
        yield return StartCoroutine(smoothLook(shotOffset(characters[currChar], 0.5f, 1f, 0), true));
        yield return StartCoroutine(moveDown());
    }

    IEnumerator normal_movedown()
    {
        yield return normal_i();
        yield return smoothLook(shotOffset(characters[currChar], 0, 1, 0), true);
        yield return StartCoroutine(moveDown());
    }

    //parts

    IEnumerator Turn_i()
    {
        yield return StartCoroutine(smoothLook(characters[currChar].position + turnoffset, true));
    }

    IEnumerator Turn()
    {
        yield return StartCoroutine(smoothLook(characters[currChar].position + turnoffset, false));
    }

    IEnumerator close_i()
    {
        yield return StartCoroutine(zoomIn(true, 2));
    }

    IEnumerator far_i()
    {
        yield return StartCoroutine(zoomOut(true, 2));
    }

    IEnumerator normal_i()
    {
        yield return null;
    }

    IEnumerator close()
    {
        yield return StartCoroutine(zoomIn(false, 2));
    }

    IEnumerator far()
    {
        yield return StartCoroutine(zoomOut(false, 2));
    }

    IEnumerator normal()
    {
        yield return null;
    }

    IEnumerator moveLeft()
    {
        StartCoroutine(posPan("left", 0.8f, 2));
        StartCoroutine(pan("left", 1f, 2));
        yield return null;
    }

    IEnumerator moveRight()
    {
        StartCoroutine(posPan("right", 0.8f, 2));
        StartCoroutine(pan("right", 1f, 2));
        yield return null;
    }

    IEnumerator moveUp()
    {
        StartCoroutine(posPan("up", 0.8f, 2f));
        StartCoroutine(pan("up", 1f, 2f));
        yield return null;
    }

    IEnumerator moveDown()
    {
        StartCoroutine(posPan("down", 0.8f, 2));
        StartCoroutine(pan("down", 1f, 2));
        yield return null;
    }

    /**********************Transitions Section**********************/

    //this is GOOD
    //will almost always use the vector3 version as it give the option for offsets, but can also have no offset
    IEnumerator smoothLook(Transform target, bool instant)
    {
        if (instant)
        {
            CameraAnchor.transform.LookAt(target);
        }
        else
        {
            Quaternion rot;
            do
            {
                Vector3 dir = target.position - transform.position;
                dir.y = 0; // keep the direction strictly horizontal
                rot = Quaternion.LookRotation(dir);
                // slerp to the desired rotation over time
                CameraAnchor.transform.rotation = Quaternion.Slerp(CameraAnchor.transform.rotation, rot, 6f * Time.deltaTime);
                Debug.Log("rotating");
                yield return new WaitForFixedUpdate();
            } while (CameraAnchor.transform.rotation != rot);
            CameraAnchor.transform.LookAt(target);
        }
    }

    //this is for looking at a specific position, the position would have to be calculated from the character's position and then offset by a certain amount
    //most likely the character position + 1 in any direction;
    IEnumerator smoothLook(Vector3 target, bool instant)
    {
        if (instant)
        {
            CameraAnchor.transform.LookAt(target);
        }
        else
        {
            Quaternion rot;
            do
            {
                Vector3 dir = target - transform.position;
                //dir.y = 0; // keep the direction strictly horizontal
                rot = Quaternion.LookRotation(dir);

                Vector3 cross = Vector3.Cross(transform.rotation * Vector3.forward, rot * Vector3.forward);
                bool turnRight = cross.y < 0;
                if (!turnRight)
                {
                    //add 360 degrees to the target to force it to turn right
                    Vector3 targetAngle = rot.eulerAngles;
                    targetAngle.y += 360;
                    rot.eulerAngles = targetAngle;
                }
                // slerp to the desired rotation over time
                CameraAnchor.transform.rotation = Quaternion.Slerp(CameraAnchor.transform.rotation, rot, 2.3f * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            } while (Quaternion.Angle(CameraAnchor.transform.rotation, rot) >= 0.74f);
            //CameraAnchor.transform.LookAt(target);
        }
    }

    //zooms the camera in a certain distance, should probably add another ratio to this based on the screen resolution
    IEnumerator zoomIn(bool instant, float distance)
    {
        if (instant)
        {
            debateCamera.transform.position += distance * debateCamera.transform.forward;
        }
        else
        {
            for (int i = 0; i < distance/0.2f; i++)
            {
                debateCamera.transform.position += 0.2f * debateCamera.transform.forward;
                yield return new WaitForSeconds(0.001f);
            }
        }
    }

    //zooms out a certain distance depending on the ratio (not added et)
    IEnumerator zoomOut(bool instant, float distance)
    {
        if (instant)
        {
            debateCamera.transform.position -= distance * debateCamera.transform.forward;
        }
        else
        {
            for (int i = 0; i < distance/0.1f; i++)
            {
                debateCamera.transform.position -= 0.1f * debateCamera.transform.forward;
                yield return new WaitForSeconds(0.001f);
            }
        }
    }


    //first iteration, angle turn offset 90, no left offset, slerp 5f * deltatime, -10f *deltatime
    //current best one, angle turn offset 30, initial 30 to the left, slerp 4f * deltatime, -1.5f * deltatime
    IEnumerator twirl_right()
    {
        Quaternion orig = debateCamera.transform.localRotation;
        Quaternion target = new Quaternion();
        target.eulerAngles = new Vector3(0, 0, debateCamera.transform.localRotation.z - 30);
        Quaternion start = debateCamera.transform.localRotation;
        start.eulerAngles = new Vector3(0, 0, debateCamera.transform.localRotation.z + 30);
        debateCamera.transform.localRotation = start;
        do
        {
            debateCamera.transform.localRotation = Quaternion.Slerp(debateCamera.transform.localRotation, target, 4f * Time.deltaTime);
            if(target.eulerAngles.z > orig.eulerAngles.z - 360)
            {
                target.eulerAngles = new Vector3(0, 0, target.eulerAngles.z - 2f * Time.deltaTime);
            }
            //debateCamera.transform.RotateAround(debateCamera.transform.position, transform.forward, 5f);
            yield return new WaitForFixedUpdate();
        }
        while (debateCamera.transform.localRotation.eulerAngles.z > orig.eulerAngles.z - 355);
        debateCamera.transform.localRotation = orig;
    }

    IEnumerator twirl_left()
    {
        Vector3 result = new Vector3(0, -30, 0);
        sideTilt(30);
        while (debateCamera.transform.localEulerAngles.y > -42)
        {
            debateCamera.transform.localEulerAngles = Vector3.Lerp(debateCamera.transform.localEulerAngles, result, 0.5f);
            yield return new WaitForFixedUpdate();
        }
    }

    /*********************************************Shots Section****************************************************/

    //x,y,z = {-1,0,1}, most likely
    //this is for rotational offsets of the camera relative to the focus character, the camera will turn to look at the offset position instead
    //of the character to simulate a positional offset, most often used to cancel out pans
    Vector3 shotOffset(Transform character, float x, float y, float z)//x indicates left(-)/right(+), y up(+)/down(-), z forward(+)/backward(-), not neccesarily the exact values, use transform.forward/right/up instead
    {
        return character.position + x * character.right + y * character.up + z * character.forward;
    }

    /*********************************************************Movement Section****************************************/

    //pan is good, need to make it dynamic, currently can pan in all 4 directions and panning forward and backward simulates zoom in/out
    //distance and speed is currently preset to 1 unit over the course of 2 seconds
    IEnumerator pan(string direction, float distance, float time)
    {
        //100 iterations, so per iteration need to move distance/100 and wait time/100
        for (int i = 0; i < 100; i++)
        {
            switch (direction)
            {
                case "right":
                    debateCamera.transform.eulerAngles += (distance / 10) * Vector3.up;
                    break;
                case "left":
                    debateCamera.transform.eulerAngles -= (distance / 10) * Vector3.up;
                    break;
                case "down":
                    debateCamera.transform.eulerAngles += (distance / 10) * Vector3.right;
                    break;
                case "up":
                    debateCamera.transform.eulerAngles -= (distance / 10) * Vector3.right;
                    break;
                case "forward":
                    debateCamera.transform.localPosition += (distance / 100) * debateCamera.transform.forward;
                    break;
                case "backward":
                    debateCamera.transform.localPosition -= (distance / 100) * debateCamera.transform.forward;
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(time / 100);
        }
        if (direction == "left" || direction == "right")
        {
            if(!GameObject.Find("Game").GetComponent<GameController>().inDebateVN())
                StartCoroutine(perpetualMotion(direction));
        }
    }

    IEnumerator posPan(string direction, float distance, float time)
    {
        //100 iterations, so per iteration need to move distance/100 and wait time/100
        for (int i = 0; i < 100; i++)
        {
            switch (direction)
            {
                case "up":
                    debateCamera.transform.localPosition += (distance / 100) * Vector3.up;
                    break;
                case "down":
                    debateCamera.transform.localPosition -= (distance / 100) * Vector3.up;
                    break;
                case "right":
                    debateCamera.transform.localPosition += (distance / 100) * Vector3.right;
                    break;
                case "left":
                    debateCamera.transform.localPosition -= (distance / 100) * Vector3.right;
                    break;
                case "forward":
                    debateCamera.transform.localPosition += (distance / 100) * debateCamera.transform.forward;
                    break;
                case "backward":
                    debateCamera.transform.localPosition -= (distance / 100) * debateCamera.transform.forward;
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(time / 100);
        }
    }

    IEnumerator perpetualMotion(string direction)
    {
        while (true)
        {
            switch (direction)
            {
                case "right":
                    CameraAnchor.transform.eulerAngles += Vector3.up * Time.deltaTime *2f;
                    break;
                case "left":
                    CameraAnchor.transform.eulerAngles -= Vector3.up * Time.deltaTime * 2f;
                    break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void setOffset(string movement)
    {
        switch (movement){
            case "moveUp":
                turnoffset = new Vector3(0f, -2f, 0);
                break;
            case "moveDown":
                turnoffset = new Vector3(0, 1.2f, 0);
                break;
            case "moveLeft":
                turnoffset = new Vector3(1.5f, 0, 0);
                break;
            case "moveRight":
                turnoffset = new Vector3(-1.5f, 0, 0);
                break;
            default:
                break;
        }
    }

    /********************************Utilities Section***************************/

    //returns a position n units away from the character at angle in the circle.
    public Vector3 NCloser(int angle, int n)
    {
        return new Vector3((distance - n) * Mathf.Sin(angle * 2 * Mathf.PI / 15), 0, (distance - n) * Mathf.Cos(angle * 2 * Mathf.PI / 15));
    }

    // right slant "\" is positive degree
    //left slant "/" is negative 
    public void cameraTilt(float degrees)
    {
        CameraAnchor.transform.localEulerAngles = new Vector3(0,0,degrees);
    }

    //deprecated, replaced by shotOffset
    public void verticalTilt(float degrees)
    {
        debateCamera.transform.localEulerAngles = new Vector3(degrees,0,0);
    }

    //deprecated, replaced by shotOffset
    public void sideTilt(float degrees)
    {
        debateCamera.transform.localEulerAngles = new Vector3(0,degrees,0);
    }

    //modifying camera position directly must be localposition, adding/subtracting can be just position
    void resetCamera()
    {
        debateCamera.transform.localPosition = new Vector3(0, 0, 7);
        debateCamera.transform.localRotation = Quaternion.identity;
    }
}
