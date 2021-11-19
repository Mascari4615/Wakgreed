using UnityEngine;

public class NotRescuedNPC : NPC
{
    public void Rescue()
    {
        DataManager.Instance.CurGameData.isNpcRescued = true;
        DataManager.Instance.SaveGameData();
        GetComponent<SpriteRenderer>().enabled = false;
        Debug.Log("Sved");
        enabled = false;
    }
}