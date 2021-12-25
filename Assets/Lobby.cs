using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Lobby : MonoBehaviour
{
    [SerializeField] private GameObject banggalTree;
    [SerializeField] private GameObject messiBar;
    [SerializeField] private GameObject dopamineLab;
    [SerializeField] private GameObject library;
    [SerializeField] private GameObject convenienceStore;
    [SerializeField] private NPC[] npcs;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private void OnEnable()
    {
        virtualCamera.m_Lens.OrthographicSize = 12;   
        GameData curGameData = DataManager.Instance.CurGameData;

        banggalTree.SetActive(curGameData.rescuedNPC[0]);
        messiBar.SetActive(curGameData.rescuedNPC[1]);
        dopamineLab.SetActive(curGameData.rescuedNPC[6] || curGameData.rescuedNPC[7] || curGameData.rescuedNPC[11] || curGameData.rescuedNPC[21]);
        library.SetActive(curGameData.rescuedNPC[8] || curGameData.rescuedNPC[10] || curGameData.rescuedNPC[17] || curGameData.rescuedNPC[18]);
        convenienceStore.SetActive(curGameData.rescuedNPC[28]);

        for (int i = 0; i < npcs.Length; i++)
            npcs[i].gameObject.SetActive(curGameData.rescuedNPC[i]);
    }
}
