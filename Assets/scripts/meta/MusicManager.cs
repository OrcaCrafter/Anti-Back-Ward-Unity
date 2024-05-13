using UnityEngine.Audio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public Sound[] songList;


    int currentSongIndex = -2;
    public Sound currentSong;

    bool focused = true;

    void Awake()
    {

        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in songList)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        currentSongIndex = Random.Range(0, songList.Length - 1);

        currentSong = songList[currentSongIndex];
        currentSong.source.Play();
    }


    void FixedUpdate () {

        if (currentSong == null || !focused)
        {
            return;
        } else if (!currentSong.source.isPlaying)
        {
            PlayMusic();
        }
        

    }
    
    public void StopMusic () {
        
        if (currentSong != null)
        {
            currentSong.source.Stop();
            currentSong = null;
        }

    }

    public void PlayMusic () {


        if (currentSong != null)
        {
            currentSong.source.Stop();
        }

        //Prevent the next song from being the same song
        int nextSongIndex = Random.Range(0, songList.Length - 2);

        if (nextSongIndex >= currentSongIndex)
        {
            nextSongIndex++;
        }

        currentSongIndex = nextSongIndex;
        currentSong = songList[nextSongIndex];
        currentSong.source.Play();
    }

    public void OnApplicationPause (bool pause)
    {

        focused = !pause;

        if (currentSong == null)
        {

            if (!pause)
            {
                PlayMusic();
            }

            return;
        }

        if (pause)
        {

            currentSong.source.Pause();

        } else
        {

            currentSong.source.UnPause();

        }
    }
}
