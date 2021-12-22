using FMOD.Studio;
using FMODUnity;
using UnityEngine;

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
    private EventInstance BgmEvent;
    private PLAYBACK_STATE pbState;
    [SerializeField] private BoolVariable isLoading;

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
        BgmEvent = RuntimeManager.CreateInstance("event:/BGM/Vendredi - Here I Am");
    }

    private void Start()
    {
        MasterVolumeLevel(DataManager.Instance.CurGameData.Volume[0]);
        SettingManager.Instance.masterSlider.value = DataManager.Instance.CurGameData.Volume[0];
        SettingManager.Instance.masterSlider.onValueChanged.AddListener(MasterVolumeLevel);

        BgmVolumeLevel(DataManager.Instance.CurGameData.Volume[1]);
        SettingManager.Instance.bgmSlider.value = DataManager.Instance.CurGameData.Volume[1];
        SettingManager.Instance.bgmSlider.onValueChanged.AddListener(BgmVolumeLevel);

        SfxVolumeLevel(DataManager.Instance.CurGameData.Volume[2]);
        SettingManager.Instance.sfxSlider.value = DataManager.Instance.CurGameData.Volume[2];
        SettingManager.Instance.sfxSlider.onValueChanged.AddListener(SfxVolumeLevel);     
    }

    private void Update()
    {
        if (isLoading.RuntimeValue == false)
        {
            BgmEvent.getPlaybackState(out pbState);
            if (pbState != PLAYBACK_STATE.PLAYING)
            {
                BgmEvent.start();
            }
        }
    }
       
    public void StopMusic() => BgmEvent.stop(STOP_MODE.ALLOWFADEOUT);

    public void PlayMusic(string musicName)
    {
        StopMusic();
        BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{musicName}");
        BgmEvent.start();
        if (UIManager.Instance != null) UIManager.Instance.SetMusicName(musicName);
    }

    public void MasterVolumeLevel(float newVolume) => 
        master.setVolume(DataManager.Instance.CurGameData.Volume[0] = newVolume);

    public void BgmVolumeLevel(float newVolume) => 
        bgm.setVolume(DataManager.Instance.CurGameData.Volume[1] = newVolume);

    public void SfxVolumeLevel(float newVolume)
    {
        sfx.setVolume(DataManager.Instance.CurGameData.Volume[2] = newVolume);

        sfxVolumeTestEvent.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            sfxVolumeTestEvent.start();
        }
    }
}