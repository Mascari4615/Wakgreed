using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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