using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField] private GameObject banggalTree;
    [SerializeField] private GameObject angel;
    [SerializeField] private GameObject secret;

    [SerializeField] private GameObject messiBar;
    [SerializeField] private GameObject rusuk;

    [SerializeField] private GameObject dopamineLab;
    [SerializeField] private GameObject seu;
    [SerializeField] private GameObject dopamine;

    [SerializeField] private GameObject library;
    [SerializeField] private GameObject leeduksoo;
    [SerializeField] private GameObject bujung;

    private void OnEnable()
    {
        GameData curGameData = DataManager.Instance.CurGameData;

        banggalTree.SetActive(curGameData.rescuedNPC[0]);
        angel.SetActive(curGameData.rescuedNPC[0]);
        secret.SetActive(curGameData.rescuedNPC[2]);

        messiBar.SetActive(curGameData.rescuedNPC[1]);
        rusuk.SetActive(curGameData.rescuedNPC[1]);

        dopamineLab.SetActive(curGameData.rescuedNPC[6] || curGameData.rescuedNPC[7]);
        dopamine.SetActive(curGameData.rescuedNPC[6]);
        seu.SetActive(curGameData.rescuedNPC[7]);

        library.SetActive(curGameData.rescuedNPC[8] || curGameData.rescuedNPC[10]);
        leeduksoo.SetActive(curGameData.rescuedNPC[8]);
        bujung.SetActive(curGameData.rescuedNPC[10]);
    }
}
