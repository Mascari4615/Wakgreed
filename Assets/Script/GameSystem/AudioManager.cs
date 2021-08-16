using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance { get { return instance;} }

    public class Sound
    {
        public string name;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;
        [Range(.1f, 3f)]
        public float pitch;
        public bool loop;

        public AudioSource source;
    }

    private Sound[] sounds;

    private AudioSource audioSource;



    private void Awake()
    {
        instance = this;

        /*
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        */

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
        //audioSource.clip = audioClip;
        //audioSource.Play();

        //Sound s = Array.Find(sounds, sound => sound.name == audioClip.name);
        // if (s == null)
        //{
// debug/log("bug");
  //      }
       // s.source.Play();
    }
}
