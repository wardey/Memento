using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpriteAtlas : MonoBehaviour {

    public Sprite[] sprites;
    public static SpriteAtlas instance;
    public Dictionary<string, Sprite> dictSprites = new Dictionary<string, Sprite>();
    // Use this for initialization
    void Awake () {
        instance = this;
        foreach (Sprite sprite in sprites)
        {
            dictSprites.Add(sprite.name, sprite);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public Sprite loadSprite(string name)
    {
        if (dictSprites.ContainsKey(name))
        {
            return dictSprites[name];
        }
        else return sprites[0]; //element 0 is default transparant sprite
    }
}
