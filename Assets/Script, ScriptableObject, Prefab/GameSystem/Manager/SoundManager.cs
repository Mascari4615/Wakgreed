using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance;} }
    private AudioSource audioSource;
    [SerializeField] private AudioClip nyang;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void Nyang()
    {
        audioSource.PlayOneShot(nyang);
    }
}
