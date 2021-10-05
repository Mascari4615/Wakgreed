using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class DataManager : MonoBehaviour
{
    protected static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<DataManager>();
                if (obj != null) { instance = obj; }
                else { instance = Create(); }
            }
            return instance;
        }
        private set { instance = value; }
    }

    [SerializeField] private ItemDataBuffer ItemDataBuffer;
    public Dictionary<int, Item> ItemDic = new();
    public WakgoodItemInventory WakgoodItemInventory;

    [SerializeField] private WeaponDataBuffer WeaponDataBuffer;
    public Dictionary<int, Weapon> WeaponDic = new();

    [SerializeField] private FoodDataBuffer FoodDataBuffer;
    public Dictionary<int, Food> FoodDic = new();
    public WakgoodFoodInventory WakgoodFoodInventory;

    public BuffRunTimeSet BuffRunTimeSet;

    public GameData curGameData;

    public static DataManager Create()
    {
        var DataManagerPrefab = Resources.Load<DataManager>("Manager_Data");
        return Instantiate(DataManagerPrefab);
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (var weapon in WeaponDataBuffer.Items) WeaponDic.Add(weapon.ID, weapon);
        foreach (var item in ItemDataBuffer.Items) ItemDic.Add(item.ID, item);
        foreach (var food in FoodDataBuffer.Items) FoodDic.Add(food.ID, food);
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

    public int GetRandomItemID()
    {
        ItemGrade itemGrade = (ItemGrade)UnityEngine.Random.Range(0, 3);
        IEnumerable<int> itemRange = Enumerable.Range(0, 0);
        if (itemGrade == ItemGrade.Common) itemRange = Enumerable.Range(0, 100);
        else if (itemGrade == ItemGrade.Uncommon) itemRange = Enumerable.Range(100, 100);
        else if (itemGrade == ItemGrade.Legendary) itemRange = Enumerable.Range(200, 100);

        var itemList = (from _itemID in ItemDic.Keys where itemRange.Contains(_itemID) select _itemID).ToList();
        int itemID = itemList[Random.Range(0, itemList.Count)];
        return itemID;
    }

    public int GetRandomItemID(ItemGrade itemGrade)
    {
        IEnumerable<int> itemRange = Enumerable.Range(0, 0);
        if (itemGrade == ItemGrade.Common) itemRange = Enumerable.Range(0, 100);
        else if (itemGrade == ItemGrade.Uncommon) itemRange = Enumerable.Range(100, 100);
        else if (itemGrade == ItemGrade.Legendary) itemRange = Enumerable.Range(200, 100);

        var itemList = (from _itemID in ItemDic.Keys where itemRange.Contains(_itemID) select _itemID).ToList();
        int itemID = itemList[Random.Range(0, itemList.Count)];
        return itemID;
    }
}

[System.Serializable]
public class GameData
{
    public bool isNPCRescued = false;

    public GameData(bool asd)
    {
        isNPCRescued = asd;
    }
}