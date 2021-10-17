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

    [Header("Item")]
    [SerializeField] private ItemDataBuffer ItemDataBuffer;
    public Dictionary<int, Item> ItemDic = new();
    public WakgoodItemInventory WakgoodItemInventory;
    public Dictionary<int, Item> CommonItemDic = new();
    public Dictionary<int, Item> UnCommonItemDic = new();
    public Dictionary<int, Item> LegendaryItemDic = new();

    [Header("Weapon")]
    [SerializeField] private WeaponDataBuffer WeaponDataBuffer;
    public Dictionary<int, Weapon> WeaponDic = new();

    [Header("Food")]
    [SerializeField] private FoodDataBuffer FoodDataBuffer;
    public Dictionary<int, Food> FoodDic = new();
    public WakgoodFoodInventory WakgoodFoodInventory;

    [Header("Mastery")]
    [SerializeField] private WakduMasteryDataBuffer WakduMasteryDataBuffer;
    public Dictionary<int, Mastery> MasteryDic = new();
    public MasteryInventory WakgoodMasteryInventory;

    [Header("Buff")]
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
        foreach (var item in ItemDataBuffer.Items)
        {
            ItemDic.Add(item.ID, item);
            if (item.grade.Equals(ItemGrade.Common)) CommonItemDic.Add(item.ID, item);
            else if (item.grade.Equals(ItemGrade.Uncommon)) UnCommonItemDic.Add(item.ID, item);
            else if (item.grade.Equals(ItemGrade.Legendary)) LegendaryItemDic.Add(item.ID, item);
        }
        foreach (var food in FoodDataBuffer.Items) FoodDic.Add(food.ID, food);
        foreach (var mastery in WakduMasteryDataBuffer.Items) MasteryDic.Add(mastery.ID, mastery);
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
        int itemID = -1;
        ItemGrade itemGrade = (ItemGrade)Random.Range(0, 3);
        if (itemGrade == ItemGrade.Common) itemID = CommonItemDic.ElementAt(Random.Range(0, CommonItemDic.Count)).Value.ID;
        else if (itemGrade == ItemGrade.Uncommon) itemID = UnCommonItemDic.ElementAt(Random.Range(0, UnCommonItemDic.Count)).Value.ID;
        else if (itemGrade == ItemGrade.Legendary) itemID = LegendaryItemDic.ElementAt(Random.Range(0, LegendaryItemDic.Count)).Value.ID;
        return itemID;
    }

    public int GetRandomItemID(ItemGrade itemGrade)
    {
        int itemID = -1;
        if (itemGrade == ItemGrade.Common) itemID = CommonItemDic.ElementAt(Random.Range(0, CommonItemDic.Count)).Value.ID;
        else if (itemGrade == ItemGrade.Uncommon) itemID = UnCommonItemDic.ElementAt(Random.Range(0, UnCommonItemDic.Count)).Value.ID;
        else if (itemGrade == ItemGrade.Legendary) itemID = LegendaryItemDic.ElementAt(Random.Range(0, LegendaryItemDic.Count)).Value.ID;
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