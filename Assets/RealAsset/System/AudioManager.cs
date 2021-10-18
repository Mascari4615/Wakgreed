using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get { return instance ?? FindObjectOfType<AudioManager>() ?? Instantiate(Resources.Load<AudioManager>("Audio_Manager")); }
        private set { instance = value; }
    }

    private Bus BGM;
    private Bus SFX;
    private Bus Master;
    private EventInstance SFXVolumeTestEvent;
    private float BGMVolume = 0.5f;
    private float SFXVolume = 0.5f;
    private float MasterVolume = 0.5f;

    public EventInstance BGMEvent;
    private PLAYBACK_STATE pbState;
    public PLAYBACK_STATE PbState { get { return pbState; } private set { pbState = value; } }


    private void Awake()
    {
        if (Instance != this) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        BGM = RuntimeManager.GetBus("bus:/Master/BGM");
        SFX = RuntimeManager.GetBus("bus:/Master/SFX");
        Master = RuntimeManager.GetBus("bus:/Master");
        SFXVolumeTestEvent = RuntimeManager.CreateInstance("event:/SFX/SFXVolumeTest");

        BGMEvent = RuntimeManager.CreateInstance("event:/BGM/Lobby");
    }

    private void Update()
    {
        BGMEvent.getPlaybackState(out pbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            BGMEvent.start();
        }
    }

    public void MasterVolumeLevel(float newMasterVoume)
    {
        MasterVolume = newMasterVoume;
        Master.setVolume(newMasterVoume);
    }

    public void BGMVolumeLevel(float newBGMVoume)
    {
        BGMVolume = newBGMVoume;
        BGM.setVolume(newBGMVoume);
    }

    public void SFXVolumeLevel(float newSFXVoume)
    {
        SFXVolume = newSFXVoume;
        SFX.setVolume(newSFXVoume);

        PLAYBACK_STATE PbState;
        SFXVolumeTestEvent.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            SFXVolumeTestEvent.start();
        }
    }
}
