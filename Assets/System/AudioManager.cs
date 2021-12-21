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
        MasterVolumeLevel(DataManager.Instance.CurGameData.Volume[0]);
        SettingManager.Instance.masterSlider.value = DataManager.Instance.CurGameData.Volume[0];
        SettingManager.Instance.masterSlider.onValueChanged.AddListener(AudioManager.Instance.MasterVolumeLevel);

        BgmVolumeLevel(DataManager.Instance.CurGameData.Volume[1]);
        SettingManager.Instance.masterSlider.value = DataManager.Instance.CurGameData.Volume[1];
        SettingManager.Instance.bgmSlider.onValueChanged.AddListener(AudioManager.Instance.BgmVolumeLevel);

        SfxVolumeLevel(DataManager.Instance.CurGameData.Volume[2]);
        SettingManager.Instance.sfxSlider.value = DataManager.Instance.CurGameData.Volume[2];
        SettingManager.Instance.sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SfxVolumeLevel);
       
    }

    private void Update()
    {
        //Debug.Log($"{DataManager.Instance.CurGameData.BGMVolume}, {DataManager.Instance.CurGameData.SfxVolume}, {DataManager.Instance.CurGameData.MasterVolume}");
        BgmEvent.getPlaybackState(out pbState);
        if (pbState != PLAYBACK_STATE.PLAYING)
        {
            BgmEvent.start();
        }
    }

    public void MasterVolumeLevel(float newMasterVolume) => master.setVolume(DataManager.Instance.CurGameData.Volume[0] = newMasterVolume);

    public void BgmVolumeLevel(float newBgmVolume) => bgm.setVolume(DataManager.Instance.CurGameData.Volume[1] = newBgmVolume);

    public void SfxVolumeLevel(float newSfxVolume)
    {
        sfx.setVolume(DataManager.Instance.CurGameData.Volume[2] = newSfxVolume);

        sfxVolumeTestEvent.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            sfxVolumeTestEvent.start();
        }
    }
}