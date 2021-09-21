using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링에 사용되는 게임 오브젝트들은 반드시 Pooling Object 스크립트나 PushObject(~) 코드를 포함한 스크립트를 컴포넌트로 가지고 있어야 함.
/// 따로 게임 오브젝트들을 비활성화시키고 풀에 넣는 관리자를 만들지 않았기 때문에, 게임 오브젝트 스스로 풀에 들어가는 코드를 가지고 있어야 함.
/// </summary>

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager instance = null;
    public static ObjectManager Instance { get { return instance; } }

    private Dictionary<string, Stack<GameObject>> poolDic = new();
    private Dictionary<string, GameObject> gameObjectDic = new();

    [System.Serializable]
    private struct PoolData
    {
        public GameObject gameObject;
        public int count;
    }
    [SerializeField] private PoolData[] poolDatas;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning($"ObjectManager Is Already Exist");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        foreach (var pD in poolDatas)
        {
            poolDic.Add(pD.gameObject.name, new());
            gameObjectDic.Add(pD.gameObject.name, pD.gameObject);

            GameObject poolGameObject = new(pD.gameObject.name);
            poolGameObject.transform.SetParent(transform);
            for (int i = 0; i < pD.count; i++) Instantiate(pD.gameObject, poolGameObject.transform).SetActive(false);
        }
    }

    public ObjectPool<PoolableObject> bulletPool = new ObjectPool<PoolableObject>();

    public Bullet bulletPrefab;

    private void Start()
    {
        bulletPool = new ObjectPool<PoolableObject>(5, () =>
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.Create(bulletPool);
            return bullet;
        });

        bulletPool.Allocate();
    }

    public void PushObject(GameObject go)
    {
        string objectName = go.name.Contains("(Clone)") ? go.name.Remove(go.name.IndexOf("("), 7) : go.name;
        poolDic[objectName].Push(go);
        go.SetActive(false);
    }

    public GameObject PopObject(string objectName, Vector3 pos)
    {
        GameObject targetObject;
        if (poolDic[objectName].Count.Equals(0))
            targetObject = Instantiate(gameObjectDic[objectName], pos, Quaternion.identity);
        else
        {
            targetObject = poolDic[objectName].Pop();
            targetObject.transform.SetPositionAndRotation(pos, Quaternion.identity);
            targetObject.SetActive(true);
        }
        return targetObject;
    }

    public GameObject PopObject(string objectName, Transform tr, bool setRot = true, bool setParent = false)
    {
        GameObject targetObject;
        if (poolDic[objectName].Count.Equals(0))
        {
            targetObject = Instantiate(gameObjectDic[objectName], tr.position, setRot ? tr.rotation : Quaternion.identity);
            if (setParent) targetObject.transform.SetParent(tr);
        }
        else
        {
            targetObject = poolDic[objectName].Pop();
            targetObject.transform.SetPositionAndRotation(tr.position, setRot ? tr.rotation : Quaternion.identity);
            if (setParent) targetObject.transform.SetParent(tr);
            targetObject.SetActive(true);
        }
        return targetObject;
    }
}

public class ObjectPool<T> where T : PoolableObject
{
    private int allocateCount;

    public delegate T Initializer();
    private Initializer initializer;

    private Stack<T> objStack;
    public List<T> objList;

    public ObjectPool(int ac, Initializer fn)
    {
        this.allocateCount = ac;
        this.initializer = fn;
        this.objStack = new Stack<T>();
        this.objList = new List<T>();
    }

    public void Allocate()
    {
        for (int index = 0; index < this.allocateCount; ++index)
        {
            this.objStack.Push(this.initializer());
        }
    }

    public T PopObject()
    {
        T obj = this.objStack.Pop();
        this.objList.Add(obj);

        obj.gameObject.SetActive(true);

        return obj;
    }

    public void PushObject(T obj)
    {
        obj.gameObject.SetActive(false);

        this.objList.Remove(obj);
        this.objStack.Push(obj);
    }

    public void Dispose()
    {
        if (this.objStack == null || this.objList == null)
            return;

        this.objList.ForEach(obj => this.objStack.Push(obj));

        while (this.objStack.Count > 0)
        {
            GameObject.Destroy(this.objStack.Pop());
        }

        this.objList.Clear();
        this.objStack.Clear();
    }
}

public class PoolableObject : MonoBehaviour
{
    protected ObjectPool<PoolableObject> pPool;

    public virtual void Create(ObjectPool<PoolableObject> pool)
    {
        pPool = pool;

        gameObject.SetActive(false);
    }

    public virtual void Dispose()
    {
        pPool.PushObject(this);
    }

    public virtual void _OnEnableContents() { }
    public virtual void _OnDisableContents() { }
}