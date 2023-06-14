using UnityEngine;

public class Lobby : MonoBehaviour
{
    public static Lobby instance;
    [SerializeField] private GameObject banggalTree;
    [SerializeField] private GameObject[] buildings;
    [SerializeField] private NPC[] npcs;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        ResetLobby();
    }

    public void ResetLobby()
    {
        GameData curGameData = DataManager.Instance.CurGameData;

        banggalTree.SetActive(curGameData.rescuedNPC[0]);

        for (int i = 0; i < buildings.Length; i++)
        {
            buildings[i].gameObject.SetActive(curGameData.buildedBuilding[i]);
        }

        foreach (NPC npc in npcs)
        {
            npc.gameObject.SetActive(curGameData.rescuedNPC[npc.ID]);
        }
    }
}