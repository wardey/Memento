using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour {

    public float flightspeed;
    public Vector3 trajectory;
    public int damage;
    public bool delay = false;

	// Update is called once per frame
	void Update () {
        if (!delay)
        {
            transform.position += transform.up * flightspeed * Time.deltaTime;
            if (transform.position.x > BHPlayerController.bound.xmax || transform.position.x < BHPlayerController.bound.xmin
                || transform.position.z > BHPlayerController.bound.zmax || transform.position.z < BHPlayerController.bound.zmin)
                Destroy(gameObject);
        }
	}

    public int getDamage()
    {
        return damage;
    }

    public void setTrajectory(Vector3 vector)
    {
        trajectory = vector;
    }

    public IEnumerator delayStart(float time)
    {
        delay = true;
        yield return new WaitForSeconds(time);
        delay = false;
    }
}
