using UnityEngine;

public class BossBoormi : InteractiveObject
{
    [SerializeField] private BossRoom bossRoom;
    public override void Interaction()
    {
        bossRoom.SummonBoss();
        gameObject.SetActive(false);
    }
}
