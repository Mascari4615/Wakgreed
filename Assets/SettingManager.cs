using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    protected static SettingManager instance;
    public static SettingManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<SettingManager>();
                if (obj != null) { instance = obj; }
                else { instance = Create(); }
            }
            return instance;
        }
        private set { instance = value; }
    }

    public GameObject settingPanel { get; private set; }
    private Slider slider1;
    private Slider slider2;
    private Slider slider3;

    public static SettingManager Create()
    {
        var AudioManagerPrefab = Resources.Load<SettingManager>("Canvas_Setting");
        return Instantiate(AudioManagerPrefab);
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        settingPanel = transform.GetChild(0).gameObject;
        slider1 = settingPanel.transform.GetChild(0).GetComponent<Slider>();
        slider2 = settingPanel.transform.GetChild(1).GetComponent<Slider>();
        slider3 = settingPanel.transform.GetChild(2).GetComponent<Slider>();
    }

    private void Start()
    {
        slider1.onValueChanged.AddListener(AudioManager.Instance.MasterVolumeLevel);
        slider2.onValueChanged.AddListener(AudioManager.Instance.BGMVolumeLevel);
        slider3.onValueChanged.AddListener(AudioManager.Instance.SFXVolumeLevel);
    }
}
