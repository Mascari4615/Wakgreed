using UnityEngine;

public class NotRescuedNPC : NPC
{
    public void Rescue()
    {
        DataManager.Instance.SaveGameData(new GameData(true));
        GetComponent<SpriteRenderer>().enabled = false;
        Debug.Log("Sved");
        enabled = false;
    }
}