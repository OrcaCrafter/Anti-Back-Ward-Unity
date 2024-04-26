using UnityEngine.Audio;
using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public Music[] music;

    public float musicTimer = 4;
    //[HideInInspector]
    public bool musicPlaying = false;
    //[HideInInspector]
    public bool newMusic = false;
    
    public Music currentSong;       //Currently used
    public Music nextSong;          //Currently used

    //...oh god
    public Music timer;             //Currently used
    public bool playingOdd;         //Currently used
    
    //Fuck this shit I'm so done
    /*public AudioSource[] musicSources;
    public int musicBPM;
    public int timeSignature;
    public int barsLength;
    private float loopPointSeconds;
    private double time;
    private int nextSource;*/


    //Be glad you didn't suffer through this
    void Start(){            //Everything uncommented here is being used
        timer = Array.Find(music, music => music.name == "Timer");
        timer.source.volume = 0;
        timer.source.Play();
        currentSong = Array.Find(music, music => music.name == "Theme"); //I get to bastardize variable anyways!
        currentSong.source.volume = 0.1f;
        currentSong.source.Play();
        playingOdd = true;
        nextSong = Array.Find(music, music => music.name == "Theme2");
        nextSong.source.volume = 0.1f;

        //------------Everything after here is either working or not used-----------------------------


        //InvokeRepeating("UpdateMusic", 0, musicTimer); If you want good stuff

        //Aaaaaaand the bad stuff
        /*loopPointSeconds = 69;
        time = AudioSettings.dspTime;
        musicSources[0].Play();
        nextSource = 1;*/
    }

    //Don't worry, there's no mercy here
    void Update(){   //Everything uncommented here is being used
        if(!timer.source.isPlaying){ //THERE'S A FUCKING ISPLAYING TAG
            timer.source.Play();
            if(playingOdd){
                nextSong.source.Play();
                playingOdd = false;
            } else{
                currentSong.source.Play();
                playingOdd = true;
            }
        }
    }


    void Awake(){

        DontDestroyOnLoad(gameObject);

        if(instance == null){
            instance = this;
        } else{
            Destroy(gameObject);
            return;
        }

        foreach(Music m in music){
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;

            m.source.volume = m.volume;
            m.source.pitch = m.pitch;
            m.source.loop = m.loop;
        }
    }
    
    public void StopMusic(){
        if(musicPlaying){
            Music c = Array.Find(music, music => music.name == currentSong.name);
            if(c == null){ 
                Debug.LogWarning("Error with current song: " + currentSong.name);
                return;
            }
            currentSong.source.Stop();
        }
    }

    public void PlayMusic (string name){

        nextSong = Array.Find(music, music => music.name == name);
        if(nextSong == null){ 
            Debug.LogWarning("Error with next song: " + name);
            return;
        }

        if(musicPlaying){
            Music c = Array.Find(music, music => music.name == currentSong.name);
            if(c == null){ 
                Debug.LogWarning("Error with current song: " + currentSong.name);
                return;
            }
        }

        newMusic = true;
    }

    public void UpdateMusic(){
        if(newMusic){
            if(musicPlaying){
                currentSong.source.Stop();
            }
            nextSong.source.Play();
            currentSong = nextSong;
            newMusic = false;
            musicPlaying = true;

        }
    }

    //And now for the most boring shit in the west because my code and music isn't "good enough"
    /*Aaaaaaand it doesn't work
    public void LoopMusic(){
        nextSong.source.Play();
    }*/
}
