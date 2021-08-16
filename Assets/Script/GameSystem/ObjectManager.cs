using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [System.Serializable] private class PoolData
    {
        public GameObject gameObject;
        public int count;
        public Queue<GameObject> queue = new Queue<GameObject>();
    }
    private static ObjectManager instance = null;
    public static ObjectManager Instance { get { return instance; } }
    private Dictionary<string, PoolData> poolDataDictionary = new Dictionary<string, PoolData>();
    [SerializeField] private PoolData[] poolDatas = new PoolData[1];
    [SerializeField] private GameObject asdf;

    void Awake()
    {
        instance = this;
        
        for (int i = 0; i < poolDatas.Length; i++)
        {
            PoolData pD = poolDatas[i];
            poolDataDictionary.Add(pD.gameObject.name, pD);

            for (int j = 0; j < pD.count; j++) Instantiate(poolDataDictionary[pD.gameObject.name].gameObject, asdf.transform).SetActive(false);
        }
    }

    public void InsertQueue(string poolObjectName, GameObject targetObject)
    {
        if (poolObjectName.Contains("(Clone)"))
            poolObjectName = poolObjectName.Remove(poolObjectName.IndexOf("("),7);

        if (!poolDataDictionary.ContainsKey(poolObjectName))
        {
            PoolData poolData = new PoolData();
            poolData.gameObject = targetObject;
            poolDataDictionary.Add(poolObjectName, poolData);
        }
        poolDataDictionary[poolObjectName].queue.Enqueue(targetObject);
        targetObject.SetActive(false);
    }
    
    public GameObject GetQueue(string poolObjectName, Vector3 createPos)
    {
        if (poolDataDictionary[poolObjectName].queue.Count == 0)
        {
            return Instantiate(poolDataDictionary[poolObjectName].gameObject, createPos, Quaternion.identity);
        }
        else
        {
            GameObject targetObject = poolDataDictionary[poolObjectName].queue.Dequeue();
            targetObject.transform.position = createPos;
            targetObject.SetActive(true);
            return targetObject;
        }
    }

    public GameObject GetQueue(string poolObjectName, Transform createTransform)
    {
        if (poolDataDictionary[poolObjectName].queue.Count == 0)
        {
            return Instantiate(poolDataDictionary[poolObjectName].gameObject, createTransform.position, createTransform.rotation);
        }
        else
        {
            GameObject targetObject = poolDataDictionary[poolObjectName].queue.Dequeue();
            targetObject.transform.SetPositionAndRotation(createTransform.position, createTransform.rotation);
            targetObject.SetActive(true);
            return targetObject;
        }
    }
}
