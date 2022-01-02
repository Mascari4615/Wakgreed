using System.Collections;
using UnityEngine;

public class BossChest : Chest
{
    protected override void Awake()
    {
        itemCount = 3;
        base.Awake();
    }

    protected override void OpenChest()
    {
        int randCount = Random.Range(3, 5 + 1);
        for (int i = 0; i < randCount; i++)
            ObjectManager.Instance.PopObject("Goldu100", transform);
        randCount = Random.Range(0, 9 + 1);
        for (int i = 0; i < randCount; i++)
            ObjectManager.Instance.PopObject("Goldu10", transform);
        randCount = Random.Range(0, 9 + 1);
        for (int i = 0; i < randCount; i++)
            ObjectManager.Instance.PopObject("Goldu", transform);
        base.OpenChest();
        StartCoroutine(SpawnPortal());
    }

    private IEnumerator SpawnPortal()
    {
        yield return new WaitForSeconds(1f);
        (StageManager.Instance.CurrentRoom as BossRoom).OpenPortal();
    }
}
