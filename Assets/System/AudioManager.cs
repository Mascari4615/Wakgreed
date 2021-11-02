using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get => instance ? instance : FindObjectOfType<AudioManager>() ?? Instantiate(Resources.Load<AudioManager>("Audio_Manager"));
        private set => instance = value;
    }

    private Bus bgm;
    private Bus sfx;
    private Bus master;
    private EventInstance sfxVolumeTestEvent;
    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;
    private float masterVolume = 0.5f;

    public EventInstance BgmEvent;
    private PLAYBACK_STATE pbState;
    public PLAYBACK_STATE PbState { get { return pbState; } private set { pbState = value; } }


    private void Awake()
    {
        if (Instance != this) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        bgm = RuntimeManager.GetBus("bus:/Master/BGM");
        sfx = RuntimeManager.GetBus("bus:/Master/SFX");
        master = RuntimeManager.GetBus("bus:/Master");
        sfxVolumeTestEvent = RuntimeManager.CreateInstance("event:/SFX/SFXVolumeTest");

        BgmEvent = RuntimeManager.CreateInstance("event:/BGM/Lobby");
    }

    private void Update()
    {
        BgmEvent.getPlaybackState(out pbState);
        if (PbState != PLAYBACK_STATE.PLAYING)
        {
            BgmEvent.start();
        }
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        masterVolume = newMasterVolume;
        master.setVolume(newMasterVolume);
    }

    public void BgmVolumeLevel(float newBgmVolume)
    {
        bgmVolume = newBgmVolume;
        bgm.setVolume(newBgmVolume);
    }

    public void SfxVolumeLevel(float newSfxVolume)
    {
        sfxVolume = newSfxVolume;
        sfx.setVolume(newSfxVolume);

        sfxVolumeTestEvent.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            sfxVolumeTestEvent.start();
        }
    }
}
