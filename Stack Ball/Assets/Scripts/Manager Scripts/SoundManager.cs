using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource audioSource;
    public bool sound = true;

    void Awake()
    {
        MakeSingleton();
        audioSource = FindObjectOfType<AudioSource>();
    }

    public void SoundOnOff()
    {
        sound = !sound;
    }

    public void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySoundFx(AudioClip clip, float volume)
    {
        if (sound)
            audioSource.PlayOneShot(clip, volume);
    }

}
