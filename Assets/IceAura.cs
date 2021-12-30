using UnityEngine;
using System.Collections;

public class IceAura : MonoBehaviour, IEffectGameObject
{
    private bool asdf = false;
    private float time1 = 0;
    private int stack = 0;
    private readonly WaitForSeconds ws01 = new(0.1f);
    private DamagingObject damagingObject;
    private GameObject Collider;

    private float GGamBBackTime = 0.5f;
    private int COUNT = 1;
    private int DEFAULT_DAMAGE;

    private void Awake()
    {
        damagingObject = transform.GetChild(0).GetComponent<DamagingObject>();
        Collider = transform.GetChild(0).gameObject;
        DEFAULT_DAMAGE = damagingObject.damage;
        StartCoroutine(GGamBBak());
    }

    private IEnumerator GGamBBak()
    {
        while (true)
        {
            Collider.SetActive(true);
            yield return new WaitForSeconds(GGamBBackTime * (1 - COUNT * 10 / 100));
            Collider.SetActive(false);
            yield return new WaitForSeconds(GGamBBackTime * (1 - COUNT * 10 / 100));
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2 + (float)stack / 1.5f, 2 + (float)stack / 1.5f, 1), 0.5f * Time.deltaTime);
        damagingObject.damage = DEFAULT_DAMAGE + stack;
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
        time1 += 1.5f;
        stack++;

        if (asdf == false)
        {
            StartCoroutine(Ang());
        }
    }

    public void Effect()
    {
        COUNT++;
    }

    public void Return()
    {
        COUNT--;
    }
}
