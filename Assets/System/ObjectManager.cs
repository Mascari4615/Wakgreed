using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 오브젝트 풀링에 사용되는 게임 오브젝트들은 반드시 Pooling Object 스크립트나 PushObject(~) 코드를 포함한 스크립트를 컴포넌트로 가지고 있어야 함.
/// 따로 게임 오브젝트들을 비활성화시키고 풀에 넣는 관리자를 만들지 않았기 때문에, 게임 오브젝트 스스로 풀에 들어가는 코드를 가지고 있어야 함.
/// </summary>

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    private readonly Dictionary<string, Stack<GameObject>> poolDic = new();
    private readonly Dictionary<string, GameObject> gameObjectDic = new();
    private readonly Dictionary<string, Transform> poolTfDic = new();

    [Serializable]
    private struct PoolData
    {
        public GameObject gameObject;
        public int count;
    }
    [FormerlySerializedAs("poolDatas")] [SerializeField] private PoolData[] poolData;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"ObjectManager Is Already Exist");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        foreach (PoolData pD in poolData)
        {
            poolDic.Add(pD.gameObject.name, new Stack<GameObject>());
            gameObjectDic.Add(pD.gameObject.name, pD.gameObject);
            GameObject poolGameObject = new(pD.gameObject.name);
            poolTfDic.Add(pD.gameObject.name, poolGameObject.transform);
            poolGameObject.transform.SetParent(transform);
            for (int i = 0; i < pD.count; i++) Instantiate(pD.gameObject, poolGameObject.transform).SetActive(false);
        }
    }

    public void PushObject(GameObject go)
    {
        string objectName = go.name.Contains("(Clone)") ? go.name.Remove(go.name.IndexOf("(", StringComparison.Ordinal), 7) : go.name;
        poolDic[objectName].Push(go);
        go.SetActive(false);
    }

    public GameObject PopObject(string objectName, Vector3 pos)
    {
        GameObject targetObject;
        if (poolDic[objectName].Count.Equals(0))
            targetObject = Instantiate(gameObjectDic[objectName], pos, Quaternion.identity, poolTfDic[objectName]);
        else
        {
            targetObject = poolDic[objectName].Pop();
            targetObject.transform.SetPositionAndRotation(pos, Quaternion.identity);
            targetObject.SetActive(true);
        }
        return targetObject;
    }

    public GameObject PopObject(string objectName, Vector3 pos, Vector3 rot)
    {
        GameObject targetObject;
        if (poolDic[objectName].Count.Equals(0))
            targetObject = Instantiate(gameObjectDic[objectName], pos, Quaternion.Euler(rot), poolTfDic[objectName]);
        else
        {
            targetObject = poolDic[objectName].Pop();
            targetObject.transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
            targetObject.SetActive(true);
        }
        return targetObject;
    }

    public GameObject PopObject(string objectName, Transform tr, bool setRot = false)
    {
        GameObject targetObject;

        if (poolDic[objectName].Count.Equals(0))
        {
            targetObject = Instantiate(gameObjectDic[objectName], tr.position, setRot ? tr.rotation : Quaternion.identity, poolTfDic[objectName]);
        }
        else
        {
            targetObject = poolDic[objectName].Pop();
            targetObject.transform.SetPositionAndRotation(tr.position, setRot ? tr.rotation : Quaternion.identity);
            targetObject.SetActive(true);
        }
        return targetObject;
    }

    public void DeactivateAll()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform pool = transform.GetChild(i);
            int objectCount = pool.childCount;

            for (int j = 0; j < objectCount; j++)
            {
                pool.GetChild(j).gameObject.SetActive(false);
            }
        }
    }

}