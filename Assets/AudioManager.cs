using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    protected static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<AudioManager>();
                if (obj != null) { instance = obj; }
                else { instance = Create(); }
            }
            return instance;
        }
        private set { instance = value; }
    }

    private Bus BGM;
    private Bus SFX;
    private Bus Master;
    private EventInstance SFXVolumeTestEvent;
    private EventInstance BGMVolumeTestEvent;

    float BGMVolume = 0.5f;
    float SFXVolume = 0.5f;
    float MasterVolume = 0.5f;

    public static AudioManager Create()
    {
        var AudioManagerPrefab = Resources.Load<AudioManager>("Audio Manager");
        return Instantiate(AudioManagerPrefab);
    }

    private void Awake()
    {
        BGM = RuntimeManager.GetBus("bus:/Master/BGM");
        SFX = RuntimeManager.GetBus("bus:/Master/SFX");
        Master = RuntimeManager.GetBus("bus:/Master");
        SFXVolumeTestEvent = RuntimeManager.CreateInstance("event:/SFX/SFXVolumeTest");
        BGMVolumeTestEvent = RuntimeManager.CreateInstance("event:/BGM/BGMVolumeTest");
    }

    public void MasterVolumeLevel (Slider newMasterVoume)
    {
        MasterVolume = newMasterVoume.value;
        Master.setVolume(newMasterVoume.value);
    }

    public void BGMVolumeLevel(Slider newBGMVoume)
    {
        BGMVolume = newBGMVoume.value;
        BGM.setVolume(newBGMVoume.value);

        PLAYBACK_STATE PbState;
        BGMVolumeTestEvent.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            BGMVolumeTestEvent.start();
        }
    }

    public void SFXVolumeLevel(Slider newSFXVoume)
    {
        SFXVolume = newSFXVoume.value;
        SFX.setVolume(newSFXVoume.value);

        PLAYBACK_STATE PbState;
        SFXVolumeTestEvent.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            SFXVolumeTestEvent.start();
        }
    }

    public void TestBGMVolume()
    {
        PLAYBACK_STATE PbState;
        BGMVolumeTestEvent.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            BGMVolumeTestEvent.start();
        }
        else
        {
            BGMVolumeTestEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void TestSFXVolume()
    {
        PLAYBACK_STATE PbState;
        SFXVolumeTestEvent.getPlaybackState(out PbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            SFXVolumeTestEvent.start();
        }
    }
}
