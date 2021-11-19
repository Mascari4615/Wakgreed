using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] private GameObject boss;

    public override void Enter() { if (IsVisited == false) IsVisited = true; }

    public void SummonBoss()
    {
        foreach (var hider in DoorHiders) hider.SetActive(true);
        foreach (var particle in DoorParticles) particle.SetActive(false);
        GameObject bossGO = Instantiate(boss, transform.Find("BossSpawnPoint"));
    }
    
    public void RoomClear()
    {
        ObjectManager.Instance.PopObject("BossChest", transform.Find("ChestPoint"));
        // 보스 클리어 연출
        transform.Find("Portal").gameObject.SetActive(true);
        foreach (var hider in DoorHiders) hider.SetActive(false);
        foreach (var particle in DoorParticles) particle.SetActive(true);
        StartCoroutine(GameManager.Instance.RoomClearSpeedWagon());
    }
}
