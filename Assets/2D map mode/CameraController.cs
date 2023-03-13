using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;

    private Vector3 offset;

    void Start()
    {
        //offset = transform.position - player.transform.position;
        offset = new Vector3(0, 0, -7f);
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
