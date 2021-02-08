using System.Collections;
using System.Collections.Generic;
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
    public Vector2 coordinate { get; private set; }
    public RoomType roomType = RoomType.Normal;
    public bool isCleared { get; private set; } = false;
    public bool isVisited { get; private set; } = false;
    public bool[] isConnectToNearbyRoom { get; private set; } = { false, false, false, false };
    [SerializeField] private int monsterCount;
    private int currentMonsterCount;
    public GameObject[] doors;
    [SerializeField] private GameObject[] doorHiders;
    [SerializeField] private Transform[] monsterSpawnPoint;
    [SerializeField] private GameObject portal;
    [SerializeField] private EnemyRunTimeSet enemyRunTimeSet;

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
        Debug.Log($"{name} : Enter");
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
            StartCoroutine(StartWave());
        }
    }

    private IEnumerator StartWave()
    {
        Debug.Log($"---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ----");
        Debug.Log($"{name} : StartWave");
        GameManager.Instance.isFighting = true;
        if (roomType == RoomType.Boss)
        { 
            monsterCount = 0;
        }
        currentMonsterCount = monsterCount;

        doorHiders[0].SetActive(true);
        doorHiders[1].SetActive(true);
        doorHiders[2].SetActive(true);
        doorHiders[3].SetActive(true);

        if (roomType == RoomType.Boss)
        {
            Vector3 summonPosition = new Vector3(monsterSpawnPoint[0].position.x, monsterSpawnPoint[0].position.y, 0);

            enemyRunTimeSet.Add(ObjectManager.Instance.GetQueue(PoolType.BossMonster, summonPosition));
            StartCoroutine(GameManager.Instance.BossSpeedWagon());     
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < monsterCount; i++)
            {
                int rand1 = Random.Range(0, monsterSpawnPoint.Length);
                int rand2 = Random.Range(0, 2);
                PoolType targetMonster = PoolType.Nothing;
                Vector3 summonPosition = new Vector3(monsterSpawnPoint[rand1].position.x, monsterSpawnPoint[rand1].position.y, 0);

                ObjectManager.Instance.GetQueue(PoolType.Summon, summonPosition);
                yield return new WaitForSeconds(0.7f);
                if (rand2 == 0)
                {
                    targetMonster = PoolType.Slime1;
                }
                else if (rand2 == 1)
                {
                    targetMonster = PoolType.Slime2;
                }
                enemyRunTimeSet.Add(ObjectManager.Instance.GetQueue(targetMonster, summonPosition));
            }   
        }
    }

    public void CheckMonsterCount()
    {
        if (roomType == RoomType.Boss) return;
        currentMonsterCount--;
        Debug.Log($"{name} : MonsterCount {currentMonsterCount}");
        if (currentMonsterCount <= 0) RoomClear();        
    }

    public void BossClear()
    {
        Debug.Log($"{name} : BossClear");
        // 보스 클리어 연출
        portal.gameObject.SetActive(true);
        RoomClear();
    }

    private void RoomClear()
    {
        Debug.Log($"{name} : RoomClear");
        Debug.Log($"---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ----");
        GameManager.Instance.isFighting = false;
        isCleared = true;

        doorHiders[0].SetActive(!isConnectToNearbyRoom[0]);
        doorHiders[1].SetActive(!isConnectToNearbyRoom[1]);
        doorHiders[2].SetActive(!isConnectToNearbyRoom[2]);
        doorHiders[3].SetActive(!isConnectToNearbyRoom[3]);       

        StartCoroutine(GameManager.Instance.RoomClearSpeedWagon());
    }
}
