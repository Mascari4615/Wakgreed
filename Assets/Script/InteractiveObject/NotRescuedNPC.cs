using UnityEngine;

public class NotRescuedNPC : NPC
{
    public override void Interaction()
    {
        base.Interaction();       
    }

    public void Rescue()
    {
        DataManager.Instance.SaveGameData(new GameData(true));
        GetComponent<SpriteRenderer>().enabled = false;
        Debug.Log("Sved");
        this.enabled = false;
    }
}
