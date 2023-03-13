using UnityEngine;
using System.Collections;

public class SFController : MonoBehaviour {

    public static SFController instance;
    public AudioSource sfPlayer;
    private AudioClip currsf;

    // Use this for initialization
    void Start()
    {
        instance = this;
        //whatever plays at the beginning, most likely nothing
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playSF(string name)
    {
        AudioClip nextSF = SoundAtlas.instance.loadSound(name);
        changeSound(nextSF);
    }

    void changeSound(AudioClip sf)
    {
        sfPlayer.mute = true;
        currsf = sf;
        sfPlayer.clip = currsf;
        sfPlayer.mute = false;
        sfPlayer.Play();
    }
}
