using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalRoom : Room
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private BoolVariable isFighting;
    private bool isCleared;
    private bool waveClearFlag;
    public int curWaveMonsterCount { get; private set; }

    public override void Enter()
    {
        base.Enter();

        if (isCleared == false)
        {
            isFighting.RuntimeValue = true;
            foreach (GameObject hider in DoorHiders)
            {
                hider.SetActive(true);
            }

            foreach (GameObject particle in DoorParticles)
            {
                particle.SetActive(false);
            }

            StartCoroutine(StartWave());
        }
    }

    private IEnumerator StartWave()
    {
        foreach (Wave wave in waves)
        {
            yield return new WaitForSeconds(2f);
            curWaveMonsterCount = wave.mobSpawnDatas.Length;
            foreach (MobSpawnData mobSpawnData in wave.mobSpawnDatas)
            {
                yield return new WaitForSeconds(mobSpawnData.spawnDelay);
                StartCoroutine(SpawnMonster(mobSpawnData));
            }

            while (!waveClearFlag)
            {
                yield return null;
            }

            waveClearFlag = false;
        }

        RoomClear();
    }

    private IEnumerator SpawnMonster(MobSpawnData mobSpawnData)
    {
        ObjectManager.Instance.PopObject("SpawnCircle", mobSpawnData.spawnPoint.position).GetComponent<Animator>()
            .SetFloat("SPEED", 1 / mobSpawnData.spawnDuration);
        yield return new WaitForSeconds(mobSpawnData.spawnDuration);
        ObjectManager.Instance.PopObject(mobSpawnData.monster.name, mobSpawnData.spawnPoint.position);
    }

    public void CheckMonsterCount()
    {
        if (--curWaveMonsterCount <= 0)
        {
            waveClearFlag = true;
        }
    }

    private void RoomClear()
    {
        isFighting.RuntimeValue = false;
        isCleared = true;
        foreach (GameObject hider in DoorHiders)
        {
            hider.SetActive(false);
        }

        foreach (GameObject particle in DoorParticles)
        {
            particle.SetActive(true);
        }

        StartCoroutine(UIManager.Instance.SpeedWagon_RoomClear());
        GameManager.Instance.OnRoomClear.Raise();

        if (Random.Range(0, 100) > 30)
        {
            Probability<string> probability = new();

            probability.Add("CommonChest", 60);
            probability.Add("UncommonChest", 30);
            probability.Add("RareChest", 9);
            probability.Add("LegendChest", 1);

            ObjectManager.Instance.PopObject(probability.Get(), transform.Find("ChestPoint"));
        }
    }

    [Serializable]
    private struct Wave
    {
        public MobSpawnData[] mobSpawnDatas;
    }

    [Serializable]
    private struct MobSpawnData
    {
        public GameObject monster;
        public Transform spawnPoint;
        public float spawnDuration;
        public float spawnDelay;
    }
}