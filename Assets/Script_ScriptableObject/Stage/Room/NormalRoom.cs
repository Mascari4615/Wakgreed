using System.Collections;
using UnityEngine;

public class NormalRoom : Room
{
    [SerializeField] private Transform chestSpawnPoint;
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

            foreach (var hider in DoorHiders) hider.SetActive(true);
            foreach (var particle in DoorParticles) particle.SetActive(false);

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
            ObjectManager.Instance.PopObject("SpawnCircle", waves[index].monsterSpawnPoint[i].position);
            yield return new WaitForSeconds(0.5f);
            //EnemyRunTimeSet.Add(ObjectManager.Instance.GetQueue(waves[index].monsters[i], waves[index].monsterSpawnPoint[i].position));
            ObjectManager.Instance.PopObject(waves[index].monsters[i].name, waves[index].monsterSpawnPoint[i].position);
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
        ObjectManager.Instance.PopObject("Chest", transform.position).GetComponent<Chest>().Initialize(ItemGrade.Common);

        if (roomType == RoomType.Boss)
        {
            // 보스 클리어 연출
            transform.GetChild(0).gameObject.SetActive(true);
        }

        GameManager.Instance.SetFighting(false);
        isCleared = true;

        foreach (var hider in DoorHiders) hider.SetActive(false);
        foreach (var particle in DoorParticles) particle.SetActive(true);

        StartCoroutine(GameManager.Instance.RoomClearSpeedWagon());
    }
}
