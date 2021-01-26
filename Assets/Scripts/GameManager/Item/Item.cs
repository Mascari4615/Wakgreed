using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemID = 0;
    public string itemName = "";
    public string itemDescription = "";
    public Sprite itemSprite = null;
    public int itemCount = 1;

    public void Effect()
    {
        Debug.Log("Effect!");
    }
}