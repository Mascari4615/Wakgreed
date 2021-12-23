using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject fakeMenu;
    [SerializeField] GameObject heumtory;
    [SerializeField] Image wakdu;

    private void Awake()
    {
        wakdu.alphaHitTestMinimumThreshold = 0.1f;

        if (DataManager.Instance.CurGameData.youtubeHi)
        {
            fakeMenu.SetActive(true);
            AudioManager.Instance.PlayMusic("Gark - Don't Shoot");
        }
        else
        {
            fakeMenu.SetActive(false);
            AudioManager.Instance.PlayMusic("Bensound - Memories");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SettingManager.Instance.OpenSetting();
    }

    public void OpenSetting() => SettingManager.Instance.OpenSetting();
    public void LoadGameScene() => SceneLoader.Instance.LoadScene("Game");
    public void StartHeumtory() { fakeMenu.SetActive(false); heumtory.SetActive(true); }
    public void Quit() => Application.Quit();
}
