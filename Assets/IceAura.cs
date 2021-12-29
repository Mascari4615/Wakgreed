using UnityEngine;
using System.Collections;

public class IceAura : MonoBehaviour, IEffectGameObject
{
    private bool asdf = false;
    private float time1 = 0;
    private float time2 = 0;
    private int stack = 0;
    private readonly WaitForSeconds ws01 = new(0.1f);
    private DamagingObject damagingObject;
    private GameObject Collider;

    private void Awake()
    {
        damagingObject = transform.GetChild(0).GetComponent<DamagingObject>();
        Collider = transform.GetChild(0).gameObject;

        StartCoroutine(GGamBBak());
    }

    private IEnumerator GGamBBak()
    {
        while (true)
        {
            Collider.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            Collider.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2 + stack, 2 + stack, 1), 0.5f * Time.deltaTime);
    }

    private IEnumerator Ang()
    {
        asdf = true;
        while (time1 > 0)
        {
            time1 -= .1f;
            yield return ws01;
        }
        stack = 0;
        asdf = false;
    }

    public void KillMonster()
    {
        time1 += 3f;
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
