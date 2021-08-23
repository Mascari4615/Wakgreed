using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    [HideInInspector] public static DataManager Instance { get { return instance; } }

    [SerializeField] private ItemDataBuffer ItemDataBuffer;
    public Dictionary<int, Item> ItemDic = new();
    public ItemInventory ItemInventory;
    public ShopKeeperItemInventory ShopKeeperItemInventory;

    [SerializeField] private WeaponDataBuffer WeaponDataBuffer;
    public Dictionary<int, Weapon> WeaponDic = new();

    public GameData curGameData;

    private void Awake()
    {
        instance = this;
        curGameData = LoadGameData();

        foreach (var weapon in WeaponDataBuffer.Items) WeaponDic.Add(weapon.ID, weapon);
        foreach (var item in ItemDataBuffer.Items) ItemDic.Add(item.ID, item);
    }

    public void SaveGameData(GameData gameData = null)
    {
        BinaryFormatter bf = new();
        FileStream stream = new(Path.Combine(Application.streamingAssetsPath, "game.wak"), FileMode.Create);

        if (gameData == null) { bf.Serialize(stream, curGameData); }
        else { bf.Serialize(stream, gameData); }
        stream.Close();
    }

    private GameData LoadGameData()
    {
        if (File.Exists(Path.Combine(Application.streamingAssetsPath, "game.wak")))
        {
            BinaryFormatter bf = new();
            FileStream stream = new(Path.Combine(Application.streamingAssetsPath, "game.wak"), FileMode.Open);

            GameData data = bf.Deserialize(stream) as GameData;

            stream.Close();
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in" + Path.Combine(Application.streamingAssetsPath, "game.wak"));
            BinaryFormatter bf = new();
            FileStream stream = new(Path.Combine(Application.streamingAssetsPath, "game.wak"), FileMode.Create);

            bf.Serialize(stream, new GameData(false));
            stream.Close();
            stream = new FileStream(Path.Combine(Application.streamingAssetsPath, "game.wak"), FileMode.Open);
            GameData data = bf.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
    }
}

[Serializable]
public class GameData
{
    public bool isNPCRescued = false;

    public GameData(bool asd)
    {
        isNPCRescued = asd;
    }
}