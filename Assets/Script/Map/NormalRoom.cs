using System.Collections;
using UnityEngine;

public class NormalRoom : Room
{
    [System.Serializable] private class Wave
    {
        [HideInInspector] public bool isCleared;
        //public PoolType[] monsters;
        public GameObject[] monsters;
        public Transform[] monsterSpawnPoint;
    }
    [SerializeField] private Wave[] waves;
    private int curMonsterCount;
    private int curWaveIndex = 0;
    private bool isCleared = false;

    public override void Enter()
    {
        if (IsVisited == false) IsVisited = true;
        if (isCleared == false)
        {
            GameManager.Instance.SetFighting(true);

            DoorHiders[0].SetActive(true);
            DoorHiders[1].SetActive(true);
            DoorHiders[2].SetActive(true);
            DoorHiders[3].SetActive(true);

            StartCoroutine(StartWave(curWaveIndex));
        }
    }

    private IEnumerator StartWave(int index)
    {
        curMonsterCount = waves[index].monsters.Length;

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < waves[index].monsters.Length; i++)
        {
            //ObjectManager.Instance.GetQueue(PoolType.Summon, waves[index].monsterSpawnPoint[i].position);
            ObjectManager.Instance.GetQueue("SpawnCircle", waves[index].monsterSpawnPoint[i].position);
            yield return new WaitForSeconds(0.5f);
            //EnemyRunTimeSet.Add(ObjectManager.Instance.GetQueue(waves[index].monsters[i], waves[index].monsterSpawnPoint[i].position));
            ObjectManager.Instance.GetQueue(waves[index].monsters[i].name, waves[index].monsterSpawnPoint[i].position);
        } 
    }

    public void CheckMonsterCount()
    {
        if (StageManager.Instance.CurrentRoom != this) return;
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
        ObjectManager.Instance.GetQueue("Item", transform.position).GetComponent<ItemGameObject>().Initialize(0);

        if (roomType == RoomType.Boss)
        {
            // 보스 클리어 연출
            transform.GetChild(0).gameObject.SetActive(true);
        }

        GameManager.Instance.SetFighting(false);
        isCleared = true;

        DoorHiders[0].SetActive(!IsConnectToNearbyRoom[0]);
        DoorHiders[1].SetActive(!IsConnectToNearbyRoom[1]);
        DoorHiders[2].SetActive(!IsConnectToNearbyRoom[2]);
        DoorHiders[3].SetActive(!IsConnectToNearbyRoom[3]);       

        StartCoroutine(GameManager.Instance.RoomClearSpeedWagon());
    }
}
