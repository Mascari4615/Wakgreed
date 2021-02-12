using System.Collections;
using UnityEngine;

public enum RoomType 
{ 
    Spawn, 
    Portal, 
    Boss, 
    Shop, 
    Interaction, 
    Normal,
}

public class Room : MonoBehaviour
{
    [System.Serializable] private class Wave
    {
        [HideInInspector] public bool isCleared;
        public PoolType[] monsters;
        public Transform[] monsterSpawnPoint;
    }
    public Vector2 coordinate { get; private set; }
    public RoomType roomType = RoomType.Normal;
    public bool isCleared { get; private set; } = false;
    public bool[] isConnectToNearbyRoom { get; private set; } = { false, false, false, false };
    private int curMonsterCount;
    public GameObject[] doors;
    [SerializeField] private GameObject[] doorHiders;
    [SerializeField] private Wave[] waves;
    private int curWaveIndex = 0;
    [SerializeField] EnemyRunTimeSet EnemyRunTimeSet;
    
    public void Initialize(Vector2 _coordinate, bool[] _isConnectToNearbyRoom)
    {
        coordinate = _coordinate;
        isConnectToNearbyRoom = _isConnectToNearbyRoom;
        transform.localPosition = coordinate * 100;

        doors[0].SetActive(isConnectToNearbyRoom[0]);
        doors[1].SetActive(isConnectToNearbyRoom[1]);
        doors[2].SetActive(isConnectToNearbyRoom[2]);
        doors[3].SetActive(isConnectToNearbyRoom[3]);
    }

    public void Enter()
    {
        if (roomType == RoomType.Spawn)
        {
            isCleared = true;
            doorHiders[0].SetActive(!isConnectToNearbyRoom[0]);
            doorHiders[1].SetActive(!isConnectToNearbyRoom[1]);
            doorHiders[2].SetActive(!isConnectToNearbyRoom[2]);
            doorHiders[3].SetActive(!isConnectToNearbyRoom[3]);
        }
        else if (isCleared == false)
        {
            GameManager.Instance.SetFighting(true);

            doorHiders[0].SetActive(true);
            doorHiders[1].SetActive(true);
            doorHiders[2].SetActive(true);
            doorHiders[3].SetActive(true);

            StartCoroutine(StartWave(curWaveIndex));
        }
    }

    private IEnumerator StartWave(int index)
    {
        curMonsterCount = waves[index].monsters.Length;

        if (roomType == RoomType.Boss)
        {
            EnemyRunTimeSet.Add(ObjectManager.Instance.GetQueue(waves[index].monsters[0], new Vector3(transform.position.x, transform.position.y, 0)));
            StartCoroutine(GameManager.Instance.BossSpeedWagon());
        }
        else
        {
            yield return new WaitForSeconds(3f);
            for (int i = 0; i < waves[index].monsters.Length; i++)
            {
                ObjectManager.Instance.GetQueue(PoolType.Summon, waves[index].monsterSpawnPoint[i].position);
                yield return new WaitForSeconds(0.7f);
                EnemyRunTimeSet.Add(ObjectManager.Instance.GetQueue(waves[index].monsters[i], waves[index].monsterSpawnPoint[i].position));
            }
        }
    }

    public void CheckMonsterCount()
    {
        if (--curMonsterCount <= 0)
        {
            waves[curWaveIndex].isCleared = true;

            if (curWaveIndex == waves.Length - 1) RoomClear();
            else
            {
                StartCoroutine(StartWave(++curWaveIndex));
            }
        }
    }

    private void RoomClear()
    {
        if (Random.Range(0, 100) < 30)
            ObjectManager.Instance.GetQueue(PoolType.Item, transform.position);

        if (roomType == RoomType.Boss)
        {
            // 보스 클리어 연출
            transform.GetChild(0).gameObject.SetActive(true);
        }

        GameManager.Instance.SetFighting(false);
        isCleared = true;

        doorHiders[0].SetActive(!isConnectToNearbyRoom[0]);
        doorHiders[1].SetActive(!isConnectToNearbyRoom[1]);
        doorHiders[2].SetActive(!isConnectToNearbyRoom[2]);
        doorHiders[3].SetActive(!isConnectToNearbyRoom[3]);       

        StartCoroutine(GameManager.Instance.RoomClearSpeedWagon());
    }
}
