using System.Collections;
using UnityEngine;

public class NormalRoom : Room
{
    [System.Serializable] private class Wave
    {
        [HideInInspector] public bool isCleared;
        public PoolType[] monsters;
        public Transform[] monsterSpawnPoint;
    }
    [SerializeField] private Wave[] waves;
    private int curMonsterCount;
    private int curWaveIndex = 0;
    [SerializeField] EnemyRunTimeSet EnemyRunTimeSet;
    private bool isCleared = false;

    public override void Enter()
    {
        if (isVisited == false) isVisited = true;
        if (isCleared == false)
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

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < waves[index].monsters.Length; i++)
        {
            ObjectManager.Instance.GetQueue(PoolType.Summon, waves[index].monsterSpawnPoint[i].position);
            yield return new WaitForSeconds(0.5f);
            EnemyRunTimeSet.Add(ObjectManager.Instance.GetQueue(waves[index].monsters[i], waves[index].monsterSpawnPoint[i].position));
        } 
    }

    public void CheckMonsterCount()
    {
        if (GameManager.Instance.currentRoom != this) return;
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
