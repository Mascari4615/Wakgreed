using UnityEngine;
using System.Collections;

public class IceAura : MonoBehaviour, IEffectGameObject
{
    private bool asdf = false;
    float time = 0;
    int stack = 0;
    WaitForSeconds ws01 = new(0.1f);
    DamagingObject damagingObject;

    private void Awake()
    {
        damagingObject = GetComponent<DamagingObject>();
    }

    private IEnumerator Ang()
    {
        asdf = true;
        while (time > 0)
        {
            time -= .1f;
            transform.localScale = new Vector3(2 + stack, 2 + stack, 1);
            yield return ws01;
        }
        stack = 0;
        asdf = false;
    }

    public void KillMonster()
    {
        time += 3f;
        stack++;

        if (asdf == false)
        {
            StartCoroutine(Ang());
        }
    }

    public void Effect()
    {
        damagingObject.damage++;
    }

    public void Return()
    {
        damagingObject.damage--;
    }
}
