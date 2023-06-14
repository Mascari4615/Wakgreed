using System.Collections;
using UnityEngine;

public class IceAura : MonoBehaviour, IEffectGameObject
{
    private readonly WaitForSeconds ws01 = new(0.1f);
    private bool asdf;
    private GameObject Collider;
    private int COUNT = 1;
    private DamagingObject damagingObject;
    private int DEFAULT_DAMAGE;

    private readonly float GGamBBackTime = 0.5f;
    private int stack;
    private float time1;

    private void Awake()
    {
        damagingObject = transform.GetChild(0).GetComponent<DamagingObject>();
        Collider = transform.GetChild(0).gameObject;
        DEFAULT_DAMAGE = damagingObject.damage;
        StartCoroutine(GGamBBak());
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2 + (stack / 3f), 2 + (stack / 3f), 1),
            0.5f * Time.deltaTime);
        damagingObject.damage = DEFAULT_DAMAGE + (int)(stack / 3f);
    }

    public void Effect()
    {
        COUNT++;
    }

    public void Return()
    {
        COUNT--;
    }

    private IEnumerator GGamBBak()
    {
        while (true)
        {
            Collider.SetActive(true);
            yield return new WaitForSeconds(GGamBBackTime * (1 - (COUNT * 10 / 100)));
            Collider.SetActive(false);
            yield return new WaitForSeconds(GGamBBackTime * (1 - (COUNT * 10 / 100)));
        }
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
}