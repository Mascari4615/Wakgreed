using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Image wakdu;

    private void Awake()
    {
        wakdu.alphaHitTestMinimumThreshold = 0.1f;
        AudioManager.Instance.PlayMusic("Bensound - Memories");     
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SettingManager.Instance.OpenSetting();
    }

    public void OpenSetting() => SettingManager.Instance.OpenSetting();
    public void LoadGameScene() => SceneLoader.Instance.LoadScene("Game");
    public void Quit() => Application.Quit();
}
