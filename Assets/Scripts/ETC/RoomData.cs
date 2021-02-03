using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public enum RoomType { Spawn, Portal, Boss, Shop, Interaction, Normal }
    public RoomType roomType = RoomType.Normal;
    [HideInInspector] public bool isCleared = false;
    [SerializeField] private int monsterCount = 0;
    private int currentMonsterCount = 0;
    public GameObject[] doors;
    public bool[] isDoorOpen = new bool[4];
    [SerializeField] private GameObject[] doorHiders;
    [SerializeField] private Transform[] monsterSpawnPoint;
    [SerializeField] private Transform[] interactionObjectSpawnPoint;
    [SerializeField] private GameObject portal;

    void OnEnable()
    {
        doors[0].SetActive(isDoorOpen[0]);
        doors[1].SetActive(isDoorOpen[1]);
        doors[2].SetActive(isDoorOpen[2]);
        doors[3].SetActive(isDoorOpen[3]);

        if (roomType == RoomType.Spawn)
        {
            isCleared = true;
            doorHiders[0].SetActive(!isDoorOpen[0]);
            doorHiders[1].SetActive(!isDoorOpen[1]);
            doorHiders[2].SetActive(!isDoorOpen[2]);
            doorHiders[3].SetActive(!isDoorOpen[3]);
        }
        else
        {
            GameManager.Instance.isFighting = true;
            if (roomType == RoomType.Boss)
            { 
                monsterCount = 0;
            }
            currentMonsterCount = monsterCount;
            StartCoroutine(StartWave());
        }  
    }

    public void CheckMonsterCount()
    {
        if (roomType == RoomType.Boss) return;

        currentMonsterCount--;
        if (currentMonsterCount <= 0) RoomClear();        
    }

    public void BossClear()
    {
        // 보스 클리어 연출
        portal.gameObject.SetActive(true);
        RoomClear();
    }

    void RoomClear()
    {
        GameManager.Instance.isFighting = false;
        isCleared = true;

        doorHiders[0].SetActive(!isDoorOpen[0]);
        doorHiders[1].SetActive(!isDoorOpen[1]);
        doorHiders[2].SetActive(!isDoorOpen[2]);
        doorHiders[3].SetActive(!isDoorOpen[3]);       

        // Vector3 summonPosition = new Vector3(transform.position.x, transform.position.y, 0);
        // ObjectManager.Instance.GetQueue(PoolType.itemBox, summonPosition);
        StartCoroutine(GameManager.Instance.RoomClearSpeedWagon());
    }

    IEnumerator StartWave()
    {
        doorHiders[0].SetActive(true);
        doorHiders[1].SetActive(true);
        doorHiders[2].SetActive(true);
        doorHiders[3].SetActive(true);

        if (roomType == RoomType.Boss)
        {
            Vector3 summonPosition = new Vector3(monsterSpawnPoint[0].position.x, monsterSpawnPoint[0].position.y, 0);

            GameManager.Instance.monsters.Add(ObjectManager.Instance.GetQueue(PoolType.BossMonster, summonPosition));
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
                GameManager.Instance.monsters.Add(ObjectManager.Instance.GetQueue(targetMonster, summonPosition));
            }   
        }
    }
}
