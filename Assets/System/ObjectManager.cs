using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링에 사용되는 게임 오브젝트들은 반드시 Pooling Object 스크립트나 PushObject(~) 코드를 포함한 스크립트를 컴포넌트로 가지고 있어야 함.
/// 따로 게임 오브젝트들을 비활성화시키고 풀에 넣는 관리자를 만들지 않았기 때문에, 게임 오브젝트 스스로 풀에 들어가는 코드를 가지고 있어야 함.
/// </summary>

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    [Serializable] private class PoolData
    {
        public GameObject gameObject;
        public Stack<GameObject> Stack = new();
        [HideInInspector] public Transform transform;
    }
    [SerializeField] private PoolData[] poolData;
    private readonly Dictionary<string, PoolData> poolDic = new();

    private void Awake()
    {
        Instance = this;

        foreach (PoolData pD in poolData)
        {
            poolDic.Add(pD.gameObject.name, pD);
            (poolDic[pD.gameObject.name].transform = new GameObject(pD.gameObject.name).transform).SetParent(transform);
            for (int i = 0; i < 10; i++) Instantiate(pD.gameObject, poolDic[pD.gameObject.name].transform).SetActive(false);
        }
    }

    public void AddPool(GameObject poolObject)
    {
        PoolData pD = new();
        pD.gameObject = poolObject;

        poolDic.Add(pD.gameObject.name, pD);
        (poolDic[pD.gameObject.name].transform = new GameObject(pD.gameObject.name).transform).SetParent(transform);
        for (int i = 0; i < 10; i++) Instantiate(pD.gameObject, poolDic[pD.gameObject.name].transform).SetActive(false);
    }

    public void PushObject(GameObject go)
    {
        string objectName = go.name.Contains("(Clone)") ? go.name.Remove(go.name.IndexOf("(", StringComparison.Ordinal), 7) : go.name;
        poolDic[objectName].Stack.Push(go);
        go.SetActive(false);
    }

    public GameObject PopObject(string objectName, Vector3 pos)
    {
        if (!poolDic.ContainsKey(objectName))
        {
            Debug.Log("No");
            return null;
        }    

        GameObject targetObject;
        if (poolDic[objectName].Stack.Count.Equals(0))
            targetObject = Instantiate(poolDic[objectName].gameObject, pos, Quaternion.identity, poolDic[objectName].transform);
        else
        {
            targetObject = poolDic[objectName].Stack.Pop();
            targetObject.transform.SetPositionAndRotation(pos, Quaternion.identity);
            targetObject.SetActive(true);
        }
        return targetObject;
    }

    public GameObject PopObject(string objectName, Vector3 pos, Vector3 rot)
    {

        if (!poolDic.ContainsKey(objectName))
        {
            Debug.Log("No");
            return null;
        }

        GameObject targetObject;
        if (poolDic[objectName].Stack.Count.Equals(0))
            targetObject = Instantiate(poolDic[objectName].gameObject, pos, Quaternion.Euler(rot), poolDic[objectName].transform);
        else
        {
            targetObject = poolDic[objectName].Stack.Pop();
            targetObject.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
            targetObject.SetActive(true);
        }
        return targetObject;
    }

    public GameObject PopObject(string objectName, Transform tr, bool setRot = false)
    {

        if (!poolDic.ContainsKey(objectName))
        {
            Debug.Log("No");
            return null;
        }


        GameObject targetObject;
        if (poolDic[objectName].Stack.Count.Equals(0))
        {
            targetObject = Instantiate(poolDic[objectName].gameObject, tr.position, setRot ? tr.rotation : Quaternion.identity, poolDic[objectName].transform);
        }
        else
        {
            targetObject = poolDic[objectName].Stack.Pop();
            targetObject.transform.SetPositionAndRotation(tr.position, setRot ? tr.rotation : Quaternion.identity);
            targetObject.SetActive(true);
        }
        return targetObject;
    }

    public void DeactivateAll()
    {
        foreach (Transform poolTransform in poolDic.Select(keyValuePair => keyValuePair.Value.transform))
        {
            for (int j = 0; j < poolTransform.childCount; j++)
                poolTransform.GetChild(j).gameObject.SetActive(false);
        }
    }
}