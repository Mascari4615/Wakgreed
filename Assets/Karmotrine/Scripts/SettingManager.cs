using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    private static SettingManager instance;
    public Slider masterSlider, bgmSlider, sfxSlider;

    private GameObject settingPanel;

    public static SettingManager Instance
    {
        get => instance
            ? instance
            : FindObjectOfType<SettingManager>() ?? Instantiate(Resources.Load<SettingManager>("Setting_Manager"));
        private set => instance = value;
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
        masterSlider = settingPanel.transform.GetChild(0).GetComponent<Slider>();
        bgmSlider = settingPanel.transform.GetChild(1).GetComponent<Slider>();
        sfxSlider = settingPanel.transform.GetChild(2).GetComponent<Slider>();
    }

    public bool Temp()
    {
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
            return true;
        }

        return false;
    }

    public void OpenSetting()
    {
        settingPanel.SetActive(true);
    }
}