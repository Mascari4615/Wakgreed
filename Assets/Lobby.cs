using UnityEngine;
using Cinemachine;

public class Lobby : MonoBehaviour
{
    [SerializeField] private GameObject banggalTree;
    [SerializeField] private GameObject messiBar;
    [SerializeField] private GameObject convenienceStore;
    [SerializeField] private NPC[] npcs;

    private void OnEnable()
    {
        GameData curGameData = DataManager.Instance.CurGameData;

        banggalTree.SetActive(curGameData.rescuedNPC[0]);
        messiBar.SetActive(curGameData.rescuedNPC[1]);
        convenienceStore.SetActive(curGameData.rescuedNPC[28]);

        foreach (var npc in npcs)
            npc.gameObject.SetActive(curGameData.rescuedNPC[npc.ID]);
    }
}
