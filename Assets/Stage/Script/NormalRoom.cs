using System.Collections;
using UnityEngine;

public class NormalRoom : Room
{
    [System.Serializable]
    private struct Wave
    {
        public MobSpawnData[] mobSpawnDatas;
    }
    [System.Serializable]
    private struct MobSpawnData
    {
        public GameObject monster;
        public Transform spawnPoint;
        public float spawnDuration;
        public float spawnDelay;
    }
    [SerializeField] private Wave[] waves;
    [HideInInspector] public int curWaveMonsterCount;
    private bool isCleared = false;
    private bool waveClearFlag = false;
    [SerializeField] private BoolVariable isFighting;

    public override void Enter()
    {
        base.Enter();

        if (isCleared == false)
        {
            isFighting.RuntimeValue = true;
            foreach (var hider in DoorHiders) hider.SetActive(true);
            foreach (var particle in DoorParticles) particle.SetActive(false);
            StartCoroutine(StartWave());
        }
    }

    private IEnumerator StartWave()
    {
        foreach (var wave in waves)
        {
            yield return new WaitForSeconds(2f);
            curWaveMonsterCount = wave.mobSpawnDatas.Length;
            UIManager.Instance.SetCurWaveText(curWaveMonsterCount);
            UIManager.Instance.SetRemainMobCount(curWaveMonsterCount);
            foreach (var mobSpawnData in wave.mobSpawnDatas)
            {
                yield return new WaitForSeconds(mobSpawnData.spawnDelay);
                StartCoroutine(SpawnMonster(mobSpawnData));              
            }
            while (!waveClearFlag) yield return null;
            waveClearFlag = false;
        }
        RoomClear();
    }

    private IEnumerator SpawnMonster(MobSpawnData mobSpawnData)
    {
        ObjectManager.Instance.PopObject("SpawnCircle", mobSpawnData.spawnPoint.position).GetComponent<Animator>().SetFloat("SPEED", 1 / mobSpawnData.spawnDuration);
        yield return new WaitForSeconds(mobSpawnData.spawnDuration);
        GameObject temp = ObjectManager.Instance.PopObject(mobSpawnData.monster.name, mobSpawnData.spawnPoint.position);

        // sDebug.Log($"Spawn Mob {temp.name} : {temp.GetInstanceID()}");
    }

    public void CheckMonsterCount() 
    {
        curWaveMonsterCount--;

        UIManager.Instance.SetRemainMobCount(curWaveMonsterCount);

        if (curWaveMonsterCount <= 0)
            waveClearFlag = true; 
    }

    private void RoomClear()
    {
        isFighting.RuntimeValue = false;
        isCleared = true;
        foreach (var hider in DoorHiders) hider.SetActive(false);
        foreach (var particle in DoorParticles) particle.SetActive(true);
        StartCoroutine(UIManager.Instance.SpeedWagon_RoomClear());

        if (Random.Range(0, 100) > 30)
        {
            Probability<string> probability = new();
            probability.Add("CommonChest", 70);
            probability.Add("UncommonChest", 35);
            probability.Add("LegendaryChest", 5);
            ObjectManager.Instance.PopObject(probability.Get(), transform.Find("ChestPoint"));
        }
    }
}
