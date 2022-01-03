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
    public bool youtubeHi = true;
    public bool[] rescuedNPC = Enumerable.Repeat(false, 30 + 1).ToArray();
    public bool[] talkedOnceNPC = Enumerable.Repeat(false, 35 + 1).ToArray();
    public bool[] killedOnceMonster = Enumerable.Repeat(false, 100 + 1).ToArray();
    public bool[] equipedOnceItem = Enumerable.Repeat(false, 100 + 1).ToArray();
    public float[] Volume = { .5f, .5f, .5f };
    public int deathCount = 0;
}

[Serializable]
public class GameData2
{
    public List<int> foods = new();
    public List<int> masteries = new();
    public List<int> items = new();
    public List<int> itemCounts = new();
    public int viewer = 0;
    public int lastStageID = 0;
    public int weapon0ID = 0;
    public int weapon1ID = 0;
    public int hp = 0;
    public int level = 0;
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

        foreach (Weapon ���� in chestSpawnWeaponDataBuffer.items)
        {
            ChestWeaponDic.Add(����.id, ����);
            switch (����.���)
            {
                case ���.�Ϲ�:
                    commonWeaponDic.Add(����.id, ����);
                    break;
                case ���.���:
                    unCommonWeaponDic.Add(����.id, ����);
                    break;
                case ���.���:
                    rareWeaponDic.Add(����.id, ����);
                    break;
                case ���.����:
                    legendWeaponDic.Add(����.id, ����);
                    break;
            }
        }

        foreach (Item item in itemDataBuffer.items)
            ItemDic.Add(item.id, item);

        foreach (Item ������ in chestSpawnItemDataBuffer.items)
        {
            ChestItemDic.Add(������.id, ������);
            switch (������.���)
            {
                case ���.�Ϲ�:
                    commonItemDic.Add(������.id, ������);
                    break;
                case ���.���:
                    unCommonItemDic.Add(������.id, ������);
                    break;
                case ���.���:
                    rareItemDic.Add(������.id, ������);
                    break;
                case ���.����:
                    legendItemDic.Add(������.id, ������);
                    break;
            }
        }

        foreach (Food food in foodDataBuffer.items) FoodDic.Add(food.id, food);
        foreach (Mastery mastery in wdMasteryBuffer.items) MasteryDic.Add(mastery.id, mastery);
        foreach (Monster monster in monsterDataBuffer.items) MonsterDic.Add(monster.ID, monster);
        foreach (Monster boss in bossDataBuffer.items) BossDic.Add(boss.ID, boss);

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

    public void SaveGameData2()
    {
        GameData2 gameData2 = new();

        gameData2.viewer = GameManager.Instance.viewer.RuntimeValue;

        int temp = wgFoodInven.Items.Count;
        for (int i = 0; i < temp; i++)
        {
            gameData2.foods.Add(wgFoodInven.Items[i].id);
        }

        temp = wgItemInven.Items.Count;
        for (int i = 0; i < temp; i++)
        {
            gameData2.items.Add(wgItemInven.Items[i].id);
            gameData2.itemCounts.Add(wgItemInven.itemCountDic[wgItemInven.Items[i].id]);
        }

        temp = wgMasteryInven.Items.Count;
        for (int i = 0; i < temp; i++)
        {
            gameData2.masteries.Add(wgMasteryInven.Items[i].id);
        }

        gameData2.lastStageID = StageManager.Instance.currentStageID;
        gameData2.weapon0ID = Wakgood.Instance.Weapon[0].id;
        gameData2.weapon1ID = Wakgood.Instance.Weapon[1].id;

        gameData2.hp = Wakgood.Instance.hpCur.RuntimeValue;
        gameData2.exp = Wakgood.Instance.exp.RuntimeValue;
        gameData2.level = Wakgood.Instance.level.RuntimeValue;

        BinaryFormatter bf = new();
        FileStream stream = new(Path.Combine(Application.streamingAssetsPath, "gameSave.wak"), FileMode.Create);

        bf.Serialize(stream, gameData2);
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
            Debug.Log("�ش� ��� ������ �������� �ʽ��ϴ�. " + Path.Combine(Application.streamingAssetsPath, "game.wak"));
            Debug.Log("�ش� ��ο� �� ������ ����ϴ�. " + Path.Combine(Application.streamingAssetsPath, "game.wak"));
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
        GetRandomItemID((���)Random.Range(0, 4));

    public int GetRandomItemID(��� ���) => ��� switch
    {
        ���.�Ϲ� => commonItemDic.ElementAt(Random.Range(0, commonItemDic.Count)).Value.id,
        ���.��� => unCommonItemDic.ElementAt(Random.Range(0, unCommonItemDic.Count)).Value.id,
        ���.��� => rareItemDic.ElementAt(Random.Range(0, rareItemDic.Count)).Value.id,
        ���.���� => legendItemDic.ElementAt(Random.Range(0, legendItemDic.Count)).Value.id,
        _ => throw new ArgumentOutOfRangeException(nameof(���), ���, null)
    };

    public int GetRandomWeaponID() => 
        GetRandomWeaponID((���)Random.Range(0, 3));

    public int GetRandomWeaponID(��� ���) => ��� switch
    {
        ���.�Ϲ� => commonWeaponDic.ElementAt(Random.Range(0, commonWeaponDic.Count)).Value.id,
        ���.��� => unCommonWeaponDic.ElementAt(Random.Range(0, unCommonWeaponDic.Count)).Value.id,
        ���.��� => rareWeaponDic.ElementAt(Random.Range(0, rareWeaponDic.Count)).Value.id,
        ���.���� => legendWeaponDic.ElementAt(Random.Range(0, legendWeaponDic.Count)).Value.id,
        _ => throw new ArgumentOutOfRangeException(nameof(���), ���, null)
    };

    public Color GetGradeColor(��� ���) => ��� switch
    {
        ���.�Ϲ� => Color.white,
        ���.��� => new(43 / 255f, 123 / 255f, 1),
        ���.��� => new(242 / 255f, 210 / 255f, 0),
        ���.���� => new (1, 0, 142 / 255f),
        _ => throw new ArgumentOutOfRangeException(nameof(���), ���, null)
    };

    private void OnApplicationQuit() => SaveGameData();
}