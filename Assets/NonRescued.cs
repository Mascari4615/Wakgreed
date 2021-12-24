using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonRescued : NPC
{
    public override void FocusOff()
    {
        base.FocusOff();
        UIManager.Instance.StartCoroutine(UIManager.Instance.SpeedWagon_Rescue());
        DataManager.Instance.CurGameData.rescuedNPC[ID] = true;
        DataManager.Instance.SaveGameData();
        gameObject.SetActive(false);
    }
}
