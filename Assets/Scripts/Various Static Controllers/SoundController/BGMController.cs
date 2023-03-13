using UnityEngine;
using System.Collections;

public class BGMController : MonoBehaviour {

    public static BGMController instance;
    public AudioSource bgmPlayer;
    public AudioSource menuPlayer;
    private AudioClip currbgm;

	// Use this for initialization
	void Start () {
        instance = this;
        //changeSong(SongAtlas.instance.loadSong((int)Random.Range(0, SongAtlas.instance.songs.Length)));
        //PlayRandomSong();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void PlayRandomSong()
    {
        AudioClip nextSong = SongAtlas.instance.loadSong((int)Random.Range(0, SongAtlas.instance.songs.Length));
        changeSong(nextSong);
        Invoke("PlayRandomSong", bgmPlayer.clip.length);
    }

    public void playSong(string name)
    {
        AudioClip nextSong = SongAtlas.instance.loadSong(name);
        StartCoroutine(songFade(nextSong));
    }

    public void pauseSong()
    {
        bgmPlayer.Pause();
    }

    public void unPauseSong()
    {
        bgmPlayer.UnPause();
    }

    void changeSong(AudioClip song)
    {
        bgmPlayer.mute = true;
        currbgm = song;
        bgmPlayer.clip = currbgm;
        bgmPlayer.mute = false;
        bgmPlayer.Play();
    }

    IEnumerator songFade(AudioClip song)
    {
        //fades out song
        float volume;
        float startTime;
        while (true)
        {
            startTime = Time.time;
            while (bgmPlayer.volume > 0)
            {
                volume = Mathf.Lerp(1f, 0f, (Time.time - startTime) / 0.5f);
                bgmPlayer.volume = volume;
                yield return null;
            }
            volume = 0f;
            bgmPlayer.volume = volume;

            changeSong(song);

            startTime = Time.time;
            while (bgmPlayer.volume < 1)
            {
                volume = Mathf.Lerp(0f, 1f, (Time.time - startTime) / 0.5f);
                bgmPlayer.volume = volume;
                yield return null;
            }
            volume = 1f;
            bgmPlayer.volume = volume;
            yield break;
        }
    }
}
