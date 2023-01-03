using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

[Serializable]
public class GameData
{
    public bool[] rescuedNPC = Enumerable.Repeat(true, 100 + 1).ToArray();
    public bool[] talkedOnceNPC = Enumerable.Repeat(true, 100 + 1).ToArray();
    public bool[] buildedBuilding = Enumerable.Repeat(true, 100 + 1).ToArray();
    public bool[] killedOnceMonster = Enumerable.Repeat(true, 100 + 1).ToArray();
    public bool[] equipedOnceItem = Enumerable.Repeat(true, 300 + 1).ToArray();
    public bool[] equipedOnceWeapon = Enumerable.Repeat(true, 300 + 1).ToArray();
    public int[] masteryStacks = Enumerable.Repeat(0, 10 + 1).ToArray();
    public int masteryStack = 1;
    public float[] Volume = { .3f, .3f, .3f };
    public int deathCount = 0;
    public int goldu = 0;
    public int level = 1;
    public int exp = 0;
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

    [Header("Item")] public ItemDataBuffer itemDataBuffer;
    [SerializeField] private ItemDataBuffer chestSpawnItemDataBuffer;
    public readonly Dictionary<int, Item> ItemDic = new();
    public readonly Dictionary<int, Item> ChestItemDic = new();
    public WakgoodItemInventory wgItemInven;
    private readonly Dictionary<int, Item> commonItemDic = new();
    private readonly Dictionary<int, Item> unCommonItemDic = new();
    private readonly Dictionary<int, Item> rareItemDic = new();
    private readonly Dictionary<int, Item> legendItemDic = new();

    [Header("Weapon")] [SerializeField] private WeaponDataBuffer weaponDataBuffer;
    [SerializeField] private WeaponDataBuffer chestSpawnWeaponDataBuffer;
    public readonly Dictionary<int, Weapon> WeaponDic = new();
    public readonly Dictionary<int, Weapon> ChestWeaponDic = new();
    private readonly Dictionary<int, Weapon> commonWeaponDic = new();
    private readonly Dictionary<int, Weapon> unCommonWeaponDic = new();
    private readonly Dictionary<int, Weapon> rareWeaponDic = new();
    private readonly Dictionary<int, Weapon> legendWeaponDic = new();

    [Header("Food")] public FoodDataBuffer foodDataBuffer;
    public readonly Dictionary<int, Food> FoodDic = new();
    public WakgoodFoodInventory wgFoodInven;

    [Header("Mastery")] public WakduMasteryDataBuffer wdMasteryBuffer;
    public readonly Dictionary<int, Mastery> MasteryDic = new();
    public MasteryInventory wgMasteryInven;

    [Header("Monster")][SerializeField] private MonsterDataBuffer monsterDataBuffer;
    [SerializeField] private MonsterDataBuffer bossDataBuffer;
    public readonly Dictionary<int, Monster> MonsterDic = new();
    public readonly Dictionary<int, Monster> BossDic = new();

    [Header("Buff")] public BuffRunTimeSet buffRunTimeSet;

    [Header("Building")] public BuildingDataBuffer buildingDataBuffer;
    public readonly Dictionary<int, Building> BuildingDic = new();

    [Header("Wakdu")] public WakduDataBuffer wakduDataBuffer;
    public readonly Dictionary<int, Wakdu> wakduDic = new();
    public GameData CurGameData { get; private set; }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        foreach (Weapon weapon in weaponDataBuffer.items)
            WeaponDic.Add(weapon.id, weapon);

        foreach (Weapon weapon in chestSpawnWeaponDataBuffer.items)
        {
            ChestWeaponDic.Add(weapon.id, weapon);
            switch (weapon.grade)
            {
                case Grade.Common:
                    commonWeaponDic.Add(weapon.id, weapon);
                    break;
                case Grade.Uncommon:
                    unCommonWeaponDic.Add(weapon.id, weapon);
                    break;
                case Grade.Rare:
                    rareWeaponDic.Add(weapon.id, weapon);
                    break;
                case Grade.Legendary:
                    legendWeaponDic.Add(weapon.id, weapon);
                    break;
            }
        }

        foreach (Item item in itemDataBuffer.items)
            ItemDic.Add(item.id, item);

        foreach (Item item in chestSpawnItemDataBuffer.items)
        {
            ChestItemDic.Add(item.id, item);
            switch (item.grade)
            {
                case Grade.Common:
                    commonItemDic.Add(item.id, item);
                    break;
                case Grade.Uncommon:
                    unCommonItemDic.Add(item.id, item);
                    break;
                case Grade.Rare:
                    rareItemDic.Add(item.id, item);
                    break;
                case Grade.Legendary:
                    legendItemDic.Add(item.id, item);
                    break;
            }
        }

        foreach (Food food in foodDataBuffer.items) FoodDic.Add(food.id, food);
        foreach (Mastery mastery in wdMasteryBuffer.items) MasteryDic.Add(mastery.id, mastery);
        foreach (Monster monster in monsterDataBuffer.items) MonsterDic.Add(monster.ID, monster);
        foreach (Monster boss in bossDataBuffer.items) BossDic.Add(boss.ID, boss);
        foreach (Building building in buildingDataBuffer.items) BuildingDic.Add(building.id, building);
        foreach (Wakdu wakdu in wakduDataBuffer.items) wakduDic.Add(wakdu.id, wakdu);

        CurGameData = LoadGameData();
        SaveGameData();
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
            Debug.Log("�ش� ��� ������ �������� �ʾ� ���� ����ϴ�. " + Path.Combine(Application.streamingAssetsPath, "game.wak"));
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

    public int GetRandomItemID() => 
        GetRandomItemID((Grade)Random.Range(0, 4));

    public int GetRandomItemID(Grade grade) => grade switch
    {
        Grade.Common => commonItemDic.ElementAt(Random.Range(0, commonItemDic.Count)).Value.id,
        Grade.Uncommon => unCommonItemDic.ElementAt(Random.Range(0, unCommonItemDic.Count)).Value.id,
        Grade.Rare => rareItemDic.ElementAt(Random.Range(0, rareItemDic.Count)).Value.id,
        Grade.Legendary => legendItemDic.ElementAt(Random.Range(0, legendItemDic.Count)).Value.id,
        _ => throw new ArgumentOutOfRangeException(nameof(grade), grade, null)
    };

    public int GetRandomWeaponID() => 
        GetRandomWeaponID((Grade)Random.Range(0, 3));

    public int GetRandomWeaponID(Grade grade) => grade switch
    {
        Grade.Common => commonWeaponDic.ElementAt(Random.Range(0, commonWeaponDic.Count)).Value.id,
        Grade.Uncommon => unCommonWeaponDic.ElementAt(Random.Range(0, unCommonWeaponDic.Count)).Value.id,
        Grade.Rare => rareWeaponDic.ElementAt(Random.Range(0, rareWeaponDic.Count)).Value.id,
        Grade.Legendary => legendWeaponDic.ElementAt(Random.Range(0, legendWeaponDic.Count)).Value.id,
        _ => throw new ArgumentOutOfRangeException(nameof(grade), grade, null)
    };

    public Color GetGradeColor(Grade grade) => grade switch
    {
        Grade.Common => Color.white,
        Grade.Uncommon => new(43 / 255f, 123 / 255f, 1),
        Grade.Rare => new(242 / 255f, 210 / 255f, 0),
        Grade.Legendary => new (1, 0, 142 / 255f),
        _ => throw new ArgumentOutOfRangeException(nameof(grade), grade, null)
    };

    private void OnApplicationQuit() => SaveGameData();
}