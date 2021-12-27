using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] private GameObject boss;

    public void SummonBoss()
    {
        foreach (var hider in DoorHiders) hider.SetActive(true);
        foreach (var particle in DoorParticles) particle.SetActive(false);
        Instantiate(boss, transform.Find("BossSpawnPoint"));
    }
    
    public void RoomClear()
    {
        ObjectManager.Instance.PopObject("BossChest", transform.Find("ChestPoint"));
        ObjectManager.Instance.PopObject("HealObject", transform.Find("ChestPoint").position + Vector3.up);
        foreach (var hider in DoorHiders) hider.SetActive(false);
        foreach (var particle in DoorParticles) particle.SetActive(true);
        StartCoroutine(UIManager.Instance.SpeedWagon_RoomClear());
    }

    public void OpenPortal() =>
        transform.Find("Portal").gameObject.SetActive(true);
}
