using System.Collections;
using UnityEngine;

public class BossMonster : Monster
{
    public IEnumerator BossHpBar()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
        }
    }
}
