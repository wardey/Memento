using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class characterImage : MonoBehaviour {

    public Image charImage;
    public string name;
    public string image;

    void start()
    {
        charImage = GetComponent<Image>();
    }

    public void remove()    //removes image from screen, fades out
    {
        StartCoroutine("fadeOut");
        charImage.sprite = SpriteAtlas.instance.loadSprite("");
        name = "";
    }

    public void setImage(string name)
    {
        this.name = name.Split('_')[0];
        image = name;
        StartCoroutine("fadeeffect");
        charImage.sprite = SpriteAtlas.instance.loadSprite(name);
    }

    //character effects should go here
    //
    //

    IEnumerator fadeeffect()        //fade effect for sprites
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
                charImage.color = spriteColor;
                yield return null;
            }
            fade = 0f;
            spriteColor.a = fade;
            charImage.color = spriteColor;

            startTime = Time.time;
            while (fade < 1f)
            {
                fade = Mathf.Lerp(0f, 1f, (Time.time - startTime) / 0.3f);
                spriteColor.a = fade;
                charImage.color = spriteColor;
                yield return null;
            }
            //Make sure it's set to exactly 1f
            fade = 1f;
            spriteColor.a = fade;
            charImage.color = spriteColor;
            yield break;
        }
    }

    IEnumerator fadeOut()        //fadeout effect for making sprite disappear
    {
        float fade = charImage.color.a;
        float startTime;
        Color spriteColor = new Color(1f, 1f, 1f, 1f);
        while (true)
        {
            startTime = Time.time;
            while (fade > 0f)
            {
                fade = Mathf.Lerp(1f, 0f, (Time.time - startTime) / 1.0f);
                spriteColor.a = fade;
                charImage.color = spriteColor;
                yield return null;
            }
            fade = 0f;
            spriteColor.a = fade;
            charImage.color = spriteColor;

            yield break;
        }
    }
}
