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

    [SerializeField] private BoolVariable isLoading;
    [SerializeField] private BoolVariable isBossing;
    [SerializeField] private BoolVariable isRealBossing;

    private Bus bgm, sfx, master;
    private EventInstance sfxVolumeTestEvent;
    private EventInstance BgmEvent;
    private PLAYBACK_STATE pbState;
    private string[] bgmTitles = { "yeppSun - 버거워", "yeppSun - 세구의 꿈", "yeppSun - 장난기 기능"};
    private string[] bossBgmTitles = { "Badassgatsby - 왁스라다", "Badassgatsby  - 왁그란 투리스모", "추르르 - Wakgood FC" };
    private int i = 0;

    private void Awake()
    {
        instance = this;

        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        bgm = RuntimeManager.GetBus("bus:/Master/BGM");
        sfx = RuntimeManager.GetBus("bus:/Master/SFX");
        master = RuntimeManager.GetBus("bus:/Master");
        sfxVolumeTestEvent = RuntimeManager.CreateInstance("event:/SFX/ETC/SFXVolumeTest");
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
        bool wakgoodCollapsed = false;
        if (Wakgood.Instance != null)
            wakgoodCollapsed = Wakgood.Instance.IsCollapsed;

        if (isRealBossing.RuntimeValue)
        {

        }
        else if (isBossing.RuntimeValue)
        {
            BgmEvent.getPlaybackState(out pbState);
            if (pbState == PLAYBACK_STATE.STOPPED)
            {
                UIManager.Instance.SetMusicName(bossBgmTitles[StageManager.Instance.currentStageID]);
                BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{bossBgmTitles[StageManager.Instance.currentStageID]}");
                BgmEvent.start();
            }
        }
        else if (isLoading.RuntimeValue == false && !wakgoodCollapsed)
        {
            BgmEvent.getPlaybackState(out pbState);
            if (pbState == PLAYBACK_STATE.STOPPED)
            {
                if (i >= bgmTitles.Length) i = 0;
                if (UIManager.Instance != null) UIManager.Instance.SetMusicName(bgmTitles[i]);
                BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{bgmTitles[i++]}");
                if (i >= bgmTitles.Length) i = 0;
                BgmEvent.start();
            }
        }
    }
       
    public void StopMusic() => BgmEvent.stop(STOP_MODE.ALLOWFADEOUT);

    public void PlayMusic(string musicName)
    {
        BgmEvent.stop(STOP_MODE.ALLOWFADEOUT);
        BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/{musicName}");
        BgmEvent.start();
    }

    public void PlayRealMusic()
    {
        UIManager.Instance.SetMusicName("Badassgatsby - 왁스라다");
        BgmEvent.stop(STOP_MODE.ALLOWFADEOUT);
        BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/Badassgatsby - 왁스라다");
        BgmEvent.start();
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