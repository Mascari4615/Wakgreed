using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    protected static SettingManager instance;
    public static SettingManager Instance
    {
        get { return instance ?? FindObjectOfType<SettingManager>() ?? Instantiate(Resources.Load<SettingManager>("Setting_Manager")); }
        private set { instance = value; }
    }

    public GameObject SettingPanel { get; private set; }
    private Slider slider1;
    private Slider slider2;
    private Slider slider3;

    private void Awake()
    {
        if (Instance != this) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        SettingPanel = transform.GetChild(0).gameObject;
        slider1 = SettingPanel.transform.GetChild(0).GetComponent<Slider>();
        slider2 = SettingPanel.transform.GetChild(1).GetComponent<Slider>();
        slider3 = SettingPanel.transform.GetChild(2).GetComponent<Slider>();
    }

    private void Start()
    {
        slider1.onValueChanged.AddListener(AudioManager.Instance.MasterVolumeLevel);
        slider2.onValueChanged.AddListener(AudioManager.Instance.BGMVolumeLevel);
        slider3.onValueChanged.AddListener(AudioManager.Instance.SFXVolumeLevel);
    }
}
