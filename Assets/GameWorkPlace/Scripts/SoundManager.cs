using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    #region singleton

    private static SoundManager _instance;
    public static SoundManager Instance{
        get{
            if(_instance == null){
                _instance = new SoundManager();

            }
            return _instance;
        }
    }
    #endregion
    private void Awake() {
         if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            
            s.source.loop = s.loop;
        }
        
    }

    public Sound[] sounds = new Sound[2];
    string LastPlayedSong;

    public void Play(string name){
        Sound s = Array.Find(sounds, sound=> sound.name == name);
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        
        s.source.Play();
    }
    public void PlaySong(string name){
        if(LastPlayedSong==null){
            Play(name);
            LastPlayedSong = name;
        }
        else{
             Sound s = Array.Find(sounds, sound=> sound.name == LastPlayedSong);
             s.source.Stop();
             LastPlayedSong = name;
             Play(name);
        }

    }
    private void Start() {
        PlaySong("Main Menu");
    }

    
}
