using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneLoader.Instance.LoadScene("Game");
    }

    public void Quit()
    {
        SceneLoader.Instance.LoadScene("Game");
        Application.Quit();
    }
}
