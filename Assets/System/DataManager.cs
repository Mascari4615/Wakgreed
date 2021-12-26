using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

[System.Serializable]
public class GameData
{
    public bool youtubeHi = true;
    public bool[] rescuedNPC = Enumerable.Repeat(false, 30 + 1).ToArray();
    public bool[] talkedOnceNPC = Enumerable.Repeat(false, 35 + 1).ToArray();
    public bool[] killedOnceMonster = Enumerable.Repeat(false, 51 + 1).ToArray();
    public bool[] killedOnceBoss = Enumerable.Repeat(false, 20 + 1).ToArray();
    public bool[] equipedOnceItem = Enumerable.Repeat(false, 51 + 1).ToArray();
    public bool[] getOnceMastery = Enumerable.Repeat(false, 50 + 1).ToArray();
    public float[] Volume = { .8f, 1, 1 };
    public int deathCount = 0;
}

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public static DataManager Instance
    {
        get => instance
            ? instance
            : FindObjectOfType<DataManager>() ?? Instantiate(Resources.Load<DataManager>("Data_Manager"));
        private set => instance = value;
    }

    [Header("Item")] [SerializeField] private ItemDataBuffer itemDataBuffer;
    public readonly Dictionary<int, Item> ItemDic = new();
    public WakgoodItemInventory wakgoodItemInventory;
    private readonly Dictionary<int, Item> commonItemDic = new();
    private readonly Dictionary<int, Item> unCommonItemDic = new();
    private readonly Dictionary<int, Item> legendaryItemDic = new();

    [Header("Weapon")] [SerializeField] private WeaponDataBuffer weaponDataBuffer;
    public readonly Dictionary<int, Weapon> WeaponDic = new();
    private Dictionary<int, Weapon> commonWeaponDic = new();
    private readonly Dictionary<int, Weapon> unCommonWeaponDic = new();
    private readonly Dictionary<int, Weapon> legendaryWeaponDic = new();

    [Header("Food")] [SerializeField] private FoodDataBuffer foodDataBuffer;
    public readonly Dictionary<int, Food> FoodDic = new();
    public WakgoodFoodInventory wakgoodFoodInventory;

    [Header("Mastery")] [SerializeField] private WakduMasteryDataBuffer wakduMasteryDataBuffer;
    public readonly Dictionary<int, Mastery> MasteryDic = new();
    public MasteryInventory wakgoodMasteryInventory;

    [Header("Monster")][SerializeField] private MonsterDataBuffer monsterDataBuffer;
    [SerializeField] private MonsterDataBuffer bossDataBuffer;
    public readonly Dictionary<int, Monster> MonsterDic = new();
    public readonly Dictionary<int, Monster> BossDic = new();

    [Header("Buff")] public BuffRunTimeSet buffRunTimeSet;

    public GameData CurGameData { get; private set; }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        CurGameData = LoadGameData();
        SaveGameData();
        
        foreach (Weapon weapon in weaponDataBuffer.items)
        {
            WeaponDic.Add(weapon.id, weapon);
            switch (weapon.grade)
            {
                case ItemGrade.Common:
                    commonWeaponDic.Add(weapon.id, weapon);
                    break;
                case ItemGrade.Uncommon:
                    unCommonWeaponDic.Add(weapon.id, weapon);
                    break;
                case ItemGrade.Legendary:
                    legendaryWeaponDic.Add(weapon.id, weapon);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        foreach (Item item in itemDataBuffer.items)
        {
            ItemDic.Add(item.id, item);
            switch (item.grade)
            {
                case ItemGrade.Common:
                    commonItemDic.Add(item.id, item);
                    break;
                case ItemGrade.Uncommon:
                    unCommonItemDic.Add(item.id, item);
                    break;
                case ItemGrade.Legendary:
                    legendaryItemDic.Add(item.id, item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        foreach (Food food in foodDataBuffer.items) FoodDic.Add(food.id, food);
        foreach (Mastery mastery in wakduMasteryDataBuffer.items) MasteryDic.Add(mastery.id, mastery);
        foreach (Monster monster in monsterDataBuffer.items) MonsterDic.Add(monster.ID, monster);
        foreach (Monster boss in bossDataBuffer.items) BossDic.Add(boss.ID, boss);
    }

    public void SaveGameData(GameData gameData = null)
    {
        BinaryFormatter bf = new();
        FileStream stream = new(Path.Combine(Application.streamingAssetsPath, "game.wak"), FileMode.Create);

        bf.Serialize(stream, gameData ?? CurGameData);
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

            bf.Serialize(stream, new GameData());
            stream.Close();
            stream = new FileStream(Path.Combine(Application.streamingAssetsPath, "game.wak"), FileMode.Open);
            GameData data = bf.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
    }

    public int GetRandomItemID() => GetRandomItemID((ItemGrade)Random.Range(0, 3));

    public int GetRandomItemID(ItemGrade itemGrade) => itemGrade switch
    {
        ItemGrade.Common => commonItemDic.ElementAt(Random.Range(0, commonItemDic.Count)).Value.id,
        ItemGrade.Uncommon => unCommonItemDic.ElementAt(Random.Range(0, unCommonItemDic.Count)).Value.id,
        ItemGrade.Legendary => legendaryItemDic.ElementAt(Random.Range(0, legendaryItemDic.Count)).Value.id,
        _ => throw new ArgumentOutOfRangeException(nameof(itemGrade), itemGrade, null)
    };

    public int GetRandomWeaponID() => GetRandomWeaponID((ItemGrade)Random.Range(0, 3));

    public int GetRandomWeaponID(ItemGrade itemGrade) => itemGrade switch
    {
        ItemGrade.Common => commonWeaponDic.ElementAt(Random.Range(0, commonWeaponDic.Count)).Value.id,
        ItemGrade.Uncommon => unCommonWeaponDic.ElementAt(Random.Range(0, unCommonWeaponDic.Count)).Value.id,
        ItemGrade.Legendary => legendaryWeaponDic.ElementAt(Random.Range(0, legendaryWeaponDic.Count)).Value.id,
        _ => throw new ArgumentOutOfRangeException(nameof(itemGrade), itemGrade, null)
    };

    private void OnApplicationQuit() => SaveGameData();
}