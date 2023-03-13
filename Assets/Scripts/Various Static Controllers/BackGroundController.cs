using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackGroundController : MonoBehaviour {

    public static BackGroundController instance;
    public Image background;

	// Use this for initialization
	void Awake () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setBackground(string name)  //change background picture
    {
        StartCoroutine("fade");
        background.sprite = SpriteAtlas.instance.loadSprite(name);
    }

    IEnumerator fade()  //slowly fades background? somekind of transition i guess
    {
        float fade = 0f;
        float startTime;
        Color spriteColor = new Color(1f, 1f, 1f, 1f);
        while (true)
        {
            startTime = Time.time;
            while (fade > 0f)
            {
                fade = Mathf.Lerp(1f, 0f, (Time.time - startTime) / 0.3f);
                spriteColor.a = fade;
                background.color = spriteColor;
                yield return null;
            }
            fade = 0f;
            spriteColor.a = fade;
            background.color = spriteColor;

            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / 0.3f);
                spriteColor.a = fade;
                background.color = spriteColor;
                yield return null;
            }
            //Make sure it's set to exactly 1f
            fade = 1f;
            spriteColor.a = fade;
            background.color = spriteColor;

            yield break;
        }
    }
}
