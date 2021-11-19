using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get => instance
            ? instance
            : FindObjectOfType<AudioManager>() ?? Instantiate(Resources.Load<AudioManager>("Audio_Manager"));
        private set => instance = value;
    }

    private Bus bgm, sfx, master;
    private EventInstance sfxVolumeTestEvent;
    public EventInstance BgmEvent;
    private PLAYBACK_STATE pbState;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        bgm = RuntimeManager.GetBus("bus:/Master/BGM");
        sfx = RuntimeManager.GetBus("bus:/Master/SFX");
        master = RuntimeManager.GetBus("bus:/Master");
        sfxVolumeTestEvent = RuntimeManager.CreateInstance("event:/SFX/SFXVolumeTest");
        BgmEvent = RuntimeManager.CreateInstance("event:/BGM/Lobby");
    }

    private void Start()
    {
        BgmVolumeLevel(DataManager.Instance.CurGameData.BGMVolume);
        SettingManager.Instance.masterSlider.value = DataManager.Instance.CurGameData.BGMVolume;
        SettingManager.Instance.bgmSlider.onValueChanged.AddListener(AudioManager.Instance.BgmVolumeLevel);

        SfxVolumeLevel(DataManager.Instance.CurGameData.SfxVolume);
        SettingManager.Instance.sfxSlider.value = DataManager.Instance.CurGameData.SfxVolume;
        SettingManager.Instance.sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SfxVolumeLevel);

        MasterVolumeLevel(DataManager.Instance.CurGameData.MasterVolume);
        SettingManager.Instance.masterSlider.value = DataManager.Instance.CurGameData.MasterVolume;
        SettingManager.Instance.masterSlider.onValueChanged.AddListener(AudioManager.Instance.MasterVolumeLevel);
    }

    private void Update()
    {
        //Debug.Log($"{DataManager.Instance.CurGameData.BGMVolume}, {DataManager.Instance.CurGameData.SfxVolume}, {DataManager.Instance.CurGameData.MasterVolume}");
        
        /*
        BgmEvent.getPlaybackState(out pbState);
        if (pbState != PLAYBACK_STATE.PLAYING)
        {
            BgmEvent.start();
        }
        */
    }

    public void MasterVolumeLevel(float newMasterVolume)
    {
        master.setVolume(DataManager.Instance.CurGameData.MasterVolume = newMasterVolume);
    }

    public void BgmVolumeLevel(float newBgmVolume)
    {
        bgm.setVolume(DataManager.Instance.CurGameData.BGMVolume = newBgmVolume);
    }

    public void SfxVolumeLevel(float newSfxVolume)
    {
        sfx.setVolume(DataManager.Instance.CurGameData.SfxVolume = newSfxVolume);

        sfxVolumeTestEvent.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            sfxVolumeTestEvent.start();
        }
    }
}