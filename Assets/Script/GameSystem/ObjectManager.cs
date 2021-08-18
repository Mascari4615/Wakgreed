using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    static ObjectManager instance = null;
    public static ObjectManager Instance { get { return instance; } }

    [System.Serializable] class PoolData
    {
        public GameObject gameObject;
        public int count;
        public Queue<GameObject> queue = new Queue<GameObject>();
    }

    Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    [SerializeField] PoolData[] poolDatas;

    void Awake()
    {
        instance = this;

        foreach (var pD in poolDatas)
        {
            poolDic.Add(pD.gameObject.name, pD);
            //Debug.Log(pD.gameObject.name);

            GameObject go = new(pD.gameObject.name);
            go.transform.SetParent(transform);

            for (int i = 0; i < pD.count; i++) Instantiate(poolDic[pD.gameObject.name].gameObject, go.transform).SetActive(false);
        }
    }

    public void InsertQueue(GameObject poolObject)
    {
        string poolName = poolObject.name.Contains("(Clone)") ? poolObject.name.Remove(poolObject.name.IndexOf("("), 7) : poolObject.name;
        //Debug.Log(poolName);

        if (!poolDic.ContainsKey(poolName))
        {
            GameObject go = new() { name = poolName };
            go.transform.SetParent(transform);

            PoolData pD = new() { gameObject = poolObject };
            poolDic.Add(poolName, pD);
        }

        poolDic[poolName].queue.Enqueue(poolObject);
        poolObject.SetActive(false);
    }
    
    public GameObject GetQueue(string poolName, Vector3 createPos, bool setParent = false)
    {
        if (poolDic[poolName].queue.Count == 0)
        {
            if (setParent) return Instantiate(poolDic[poolName].gameObject, createPos, Quaternion.identity, transform.Find(poolName));
            else return Instantiate(poolDic[poolName].gameObject, createPos, Quaternion.identity);        }
        else
        {
            GameObject targetObject = poolDic[poolName].queue.Dequeue();
            targetObject.transform.SetPositionAndRotation(createPos, Quaternion.identity);
            targetObject.SetActive(true);
            return targetObject;
        }
    }

    public GameObject GetQueue(string poolName, Transform transform, bool setParent = false)
    {
        if (poolDic[poolName].queue.Count == 0)
        {
            if (setParent) return Instantiate(poolDic[poolName].gameObject, transform.position, transform.rotation, transform.Find(poolName));
            else return Instantiate(poolDic[poolName].gameObject, transform.position, transform.rotation);
        }
        else
        {
            GameObject targetObject = poolDic[poolName].queue.Dequeue();
            targetObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            if (setParent) targetObject.transform.SetParent(transform);
            targetObject.SetActive(true);
            return targetObject;
        }
    }
}
