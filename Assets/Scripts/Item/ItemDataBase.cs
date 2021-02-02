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
}

public class ItemDataBase : MonoBehaviour
{
    private static ItemDataBase instance;
    [HideInInspector] public static ItemDataBase Instance { get { return instance; } }

    public Item[] items;

    void Awake()
    {
        instance = this;
    }
}