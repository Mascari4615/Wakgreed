using UnityEngine;

public class BossRoom : Room
{
    //[SerializeField] private PoolType boss;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform bossSpawnPoint;

    public override void Enter()
    {
        if (IsVisited == false) IsVisited = true;
        DoorHiders[0].SetActive(!IsConnectToNearbyRoom[0]);
        DoorHiders[1].SetActive(!IsConnectToNearbyRoom[1]);
        DoorHiders[2].SetActive(!IsConnectToNearbyRoom[2]);
        DoorHiders[3].SetActive(!IsConnectToNearbyRoom[3]);
    }

    public void SummonBoss()
    {
        GameManager.Instance.SetFighting(true);

        DoorHiders[0].SetActive(true);
        DoorHiders[1].SetActive(true);
        DoorHiders[2].SetActive(true);
        DoorHiders[3].SetActive(true);

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

        DoorHiders[0].SetActive(!IsConnectToNearbyRoom[0]);
        DoorHiders[1].SetActive(!IsConnectToNearbyRoom[1]);
        DoorHiders[2].SetActive(!IsConnectToNearbyRoom[2]);
        DoorHiders[3].SetActive(!IsConnectToNearbyRoom[3]);

        StartCoroutine(GameManager.Instance.RoomClearSpeedWagon());
    }
}
