using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
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
    Item,
    BBolBBol
}

public class ObjectManager : MonoBehaviour
{
    [System.Serializable] private class PoolData
    {
        public PoolType type;
        public GameObject gameObject;
        public Queue<GameObject> queue = new Queue<GameObject>();
    }
    private static ObjectManager instance = null;
    public static ObjectManager Instance { get { return instance; } }
    private Dictionary<PoolType, PoolData> poolDataDictionary = new Dictionary<PoolType, PoolData>();
    [SerializeField] private PoolData[] poolDatas = new PoolData[1];

    void Awake()
    {
        instance = this;

        for (int i = 0; i < poolDatas.Length; i++)
        {
            PoolData pD = poolDatas[i];
            poolDataDictionary.Add(pD.type, pD);
        }
    }

    private void AddQueue(PoolType poolObjectType)
    {
        GameObject go = Instantiate(poolDataDictionary[poolObjectType].gameObject);
        InsertQueue(poolObjectType, go);
    }

    public void InsertQueue(PoolType poolObjectType, GameObject targetObject)
    {
        poolDataDictionary[poolObjectType].queue.Enqueue(targetObject);
        targetObject.SetActive(false); 
    }
    
    public GameObject GetQueue(PoolType poolObjectType, Vector3 createPos)
    {
        if (poolDataDictionary[poolObjectType].queue.Count == 0) AddQueue(poolObjectType);

        GameObject targetObject = poolDataDictionary[poolObjectType].queue.Dequeue();
        targetObject.transform.position = createPos;
        targetObject.SetActive(true);
        return targetObject;
    }

    public GameObject GetQueue(PoolType poolObjectType, Transform createTransform)
    {
        if (poolDataDictionary[poolObjectType].queue.Count == 0) AddQueue(poolObjectType);

        GameObject targetObject = poolDataDictionary[poolObjectType].queue.Dequeue();
        targetObject.transform.SetPositionAndRotation(createTransform.position, createTransform.rotation);
        targetObject.SetActive(true);
        return targetObject;
    }

    public GameObject GetQueue(PoolType poolObjectType, Vector3 createPos, string text, string type)
    {
        if (poolDataDictionary[poolObjectType].queue.Count == 0) AddQueue(poolObjectType);

        GameObject targetObject = poolDataDictionary[poolObjectType].queue.Dequeue();
        targetObject.transform.position = createPos;
        targetObject.GetComponent<DamageText>().text = text;
        targetObject.GetComponent<DamageText>().type = type;
        targetObject.SetActive(true);
        return targetObject;
    }
}
