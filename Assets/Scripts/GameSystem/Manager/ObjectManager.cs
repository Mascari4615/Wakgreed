using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    Nothing,
    AnimatedText,
    KnightSwordSlash,
    ArcherArrow,
    Slime1,
    Slime2,
    BossMonster,
    Exp,
    Nyang,
    Smoke,
    Summon,
    Item
}

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager instance = null;
    public static ObjectManager Instance { get { return instance; } }
    private Dictionary<PoolType, Queue<GameObject>> poolDataDictionary = new Dictionary<PoolType, Queue<GameObject>>(); 
    [System.Serializable] private class PoolData
    {
        [SerializeField] private PoolType _poolObjectType = PoolType.Nothing;
        [SerializeField] private GameObject _poolObject = null;
        [SerializeField] private int _poolObjectAmount = 0;
        [SerializeField] private Queue<GameObject> _poolObjectQueue = new Queue<GameObject>();

        public PoolType poolObjectType { get { return _poolObjectType; } }
        public GameObject poolObject { get { return _poolObject; } }
        public int poolObjectAmount { get { return _poolObjectAmount; } }
        public Queue<GameObject> poolObjectQueue { get { return _poolObjectQueue; } }
    }
    [SerializeField] private PoolData[] poolDatas = null;
    public Transform monsterParent = null;
    [SerializeField] private Transform elseParent = null;

    void Awake()
    {
        instance = this;

        for (int i = 0; i < poolDatas.Length; i++)
        {
            PoolData pD = poolDatas[i];
            poolDataDictionary.Add(pD.poolObjectType, pD.poolObjectQueue);

            if (pD.poolObjectType == PoolType.Slime1 || pD.poolObjectType == PoolType.Slime2 || pD.poolObjectType == PoolType.BossMonster)
            {
                for (int j = 0; j < pD.poolObjectAmount; j++)
                {
                    GameObject targetObject = Instantiate(pD.poolObject);
                    targetObject.transform.SetParent(monsterParent);
                    InsertQueue(pD.poolObjectType, targetObject);
                }   
            }
            else
            {
                for (int j = 0; j < pD.poolObjectAmount; j++)
                {
                    GameObject targetObject = Instantiate(pD.poolObject);
                    targetObject.transform.SetParent(elseParent);
                    InsertQueue(pD.poolObjectType, targetObject);
                }  
            }
        }
    }

    private void AddQueue(PoolType poolObjectType)
    {
        for (int i = 0; i < poolDatas.Length; i++)
        {
            if (poolDatas[i].poolObjectType == poolObjectType)
            {
                GameObject go = Instantiate(poolDatas[i].poolObject);
                go.transform.SetParent(elseParent);
                InsertQueue(poolObjectType, go);
                break;
            }
        }
    }

    public void InsertQueue(PoolType poolObjectType, GameObject targetObject)
    {
        poolDataDictionary[poolObjectType].Enqueue(targetObject);
        targetObject.SetActive(false); 
    }

    public void InsertAll()
    {
        for (int i = 0; i < monsterParent.transform.childCount; i++)
        {
            InsertQueue(monsterParent.transform.GetChild(i).GetComponent<PoolingObject>().poolType, monsterParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < elseParent.transform.childCount; i++)
        {
            InsertQueue(elseParent.transform.GetChild(i).GetComponent<PoolingObject>().poolType, elseParent.transform.GetChild(i).gameObject);
        }
    }

    public GameObject GetQueue(PoolType poolObjectType, Transform createTransform)
    {
        if (poolDataDictionary[poolObjectType].Count == 0) AddQueue(poolObjectType);

        GameObject targetObject = poolDataDictionary[poolObjectType].Dequeue();
        targetObject.transform.position = createTransform.position;
        targetObject.transform.rotation = createTransform.rotation;
        targetObject.SetActive(true);
        return targetObject;
    }
    
    public GameObject GetQueue(PoolType poolObjectType, Vector3 createPos)
    {
        if (poolDataDictionary[poolObjectType].Count == 0) AddQueue(poolObjectType);

        GameObject targetObject = poolDataDictionary[poolObjectType].Dequeue();
        targetObject.transform.position = createPos;
        targetObject.SetActive(true);
        return targetObject;
    }

    public GameObject GetQueue(PoolType poolObjectType, Vector3 createPos, string text, string type)
    {
        if (poolDataDictionary[poolObjectType].Count == 0) AddQueue(poolObjectType);

        GameObject targetObject = poolDataDictionary[poolObjectType].Dequeue();
        targetObject.transform.position = createPos;
        targetObject.GetComponent<DamageText>().text = text;
        targetObject.GetComponent<DamageText>().type = type;
        targetObject.SetActive(true);
        return targetObject;
    }
}
