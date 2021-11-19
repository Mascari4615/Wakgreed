using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    private static SettingManager instance;
    public static SettingManager Instance
    {
        get => instance ? instance : FindObjectOfType<SettingManager>() ?? Instantiate(Resources.Load<SettingManager>("Setting_Manager"));
        private set => instance = value;
    }

    public GameObject SettingPanel { get; private set; }
    public Slider masterSlider, bgmSlider, sfxSlider;

    private void Awake()
    {
        if (Instance != this) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        SettingPanel = transform.GetChild(0).gameObject;
        masterSlider = SettingPanel.transform.GetChild(0).GetComponent<Slider>();
        bgmSlider = SettingPanel.transform.GetChild(1).GetComponent<Slider>();
        sfxSlider = SettingPanel.transform.GetChild(2).GetComponent<Slider>();
    }
}
