using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _musicSource;
    private AudioSource _soundSource;

    private float SoundVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("SoundVolume",1.0f);
        }
        set
        {
            _soundSource.volume = value;
            PlayerPrefs.SetFloat("SoundVolume", value);
        }
    }

    private float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("MusicVolumn",1.0f);
        }
        set
        {
            _musicSource.volume = value;
            PlayerPrefs.SetFloat("MusicVolumn", value);
        }
    }

    private void Awake()
    {
        _musicSource = this.gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;

        _soundSource = this.gameObject.AddComponent<AudioSource>();
        _soundSource.playOnAwake = false;
        _soundSource.loop = false;
    }



    public void PlayMusic(string musicName)
    {
        if (MusicVolume < 0.1f)
            return;
        string curMusic = "";
        if (_musicSource.clip != null)
        {
            curMusic = _musicSource.clip.name;
        }
        if (curMusic == musicName)
        {
            _musicSource.Play();
            return;
        }
        Manager.Resource.LoadMusic(musicName, (UnityEngine.Object obj) =>
         {
             _musicSource.clip = obj as AudioClip;
             _musicSource.Play();
         });
    }

    public void StopMusic(string musicName)
    {
        _musicSource.Stop();
    }

    public void PauseMusic()
    {
        _musicSource.Pause();
    }

    public void ContinueMusic()
    {
        _musicSource.UnPause();
    }

    public void PlaySound(string soundName)
    {
        if (SoundVolume < 0.1f)
            return;
        Manager.Resource.LoadSound(soundName, (UnityEngine.Object obj) =>
         {
             _soundSource.PlayOneShot(obj as AudioClip);
         });
    }

    public void SetMusicVolume(float volume)
    {
        this.MusicVolume = volume;
        if (volume < 0.1f)
        {
            PauseMusic();
        }
        else
        {
            ContinueMusic();
        }
    }

    public void SetSoundVolume(float volume)
    {
        this.SoundVolume = volume;
    }

    public float GetMusicVolume()
    {
        return MusicVolume;
    }

    public float GetSoundVolume()
    {
        return SoundVolume;
    }
}
