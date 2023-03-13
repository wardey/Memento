using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

    public GameObject arrow;

    void OnEnable()
    {
        StartCoroutine("flash");
    }

    IEnumerator flash()
    {
        while (true)
        {
            arrow.SetActive(true);
            yield return new WaitForSeconds(.5f);
            arrow.SetActive(false);
            yield return new WaitForSeconds(.25f);
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
