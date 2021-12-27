using System.Collections;
using UnityEngine;

public class BossChest : Chest
{
    protected override void OpenChest()
    {
        base.OpenChest();
        StartCoroutine(SpawnPortal());
    }

    private IEnumerator SpawnPortal()
    {
        yield return new WaitForSeconds(1f);
        (StageManager.Instance.CurrentRoom as BossRoom).OpenPortal();
    }
}
