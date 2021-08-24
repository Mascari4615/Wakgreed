using UnityEngine;

public class BossRoom : Room
{
    //[SerializeField] private PoolType boss;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform bossSpawnPoint;

    public override void Enter()
    {
        if (IsVisited == false) IsVisited = true;
    }

    public void SummonBoss()
    {
        GameManager.Instance.SetFighting(true);

        foreach (var hider in DoorHiders) hider.SetActive(true);
        foreach (var particle in DoorParticles) particle.SetActive(false);

        //GameObject bossGO = ObjectManager.Instance.GetQueue(boss, bossSpawnPoint.position);
        GameObject bossGO = ObjectManager.Instance.GetQueue(boss.name, bossSpawnPoint.position);

        StartCoroutine(UIManager.Instance.SpeedWagon_Boss(bossGO));
    }

    public void CheckMonsterCount()
    {
        if (StageManager.Instance.CurrentRoom != this) return;
        // 페이즈 체크
        RoomClear();
    }

    private void RoomClear()
    {
        if (Random.Range(0, 100) < 30)
            ObjectManager.Instance.GetQueue("Item", transform.position);

        // 보스 클리어 연출
        transform.Find("Portal").gameObject.SetActive(true);

        GameManager.Instance.SetFighting(false);

        foreach (var hider in DoorHiders) hider.SetActive(false);
        foreach (var particle in DoorParticles) particle.SetActive(true);

        StartCoroutine(GameManager.Instance.RoomClearSpeedWagon());
    }
}
