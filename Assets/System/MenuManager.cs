using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject fakeMenu;
    [SerializeField] GameObject heumtory;

    private void Awake()
    {
        if (DataManager.Instance.CurGameData.youtubeHi)
        {
            DataManager.Instance.CurGameData.youtubeHi = false;
            DataManager.Instance.SaveGameData();
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
    public void LoadTutorialScene() { fakeMenu.SetActive(false); heumtory.SetActive(true); }

    public void Quit()
    {
        SceneLoader.Instance.LoadScene("Game");
        Application.Quit();
    }
}
