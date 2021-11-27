using System.Collections;
using UnityEngine;

public class Banshee : NormalMonster
{
    private bool bRecognizeWakgood = false;
    [SerializeField] private BulletMove[] notes;
    private static readonly int attack = Animator.StringToHash("ATTACK");

    protected override void OnEnable()
    {
        bRecognizeWakgood = false;
        base.OnEnable();
        StartCoroutine(CheckDistance());
    }

    private IEnumerator CheckDistance()
    {
        WaitForSeconds ws01 = new(0.1f);
        while (true)
        {
            if (!bRecognizeWakgood)
            {
                if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) < 15)
                {
                    bRecognizeWakgood = true;
                    StartCoroutine(Attack());
                }
            }
            else
            {
                SpriteRenderer.flipX = transform.position.x < Wakgood.Instance.transform.position.x;
            }
            yield return ws01;
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds ws2 = new(2f), ws5 = new(5f);
        
        while (true)
        {
            yield return ws2;
            yield return StartCoroutine(Casting(1f));
            Cry();
            Animator.SetTrigger(attack);
            yield return ws5;
        }
    }

    private void Cry()
    {
        Vector3 direction = Vector3.zero;
        float temp = 360f / notes.Length;

        for (int i = 0; i < notes.Length; i++)
        {
            direction.Set(Mathf.Cos(temp * i * Mathf.Deg2Rad), Mathf.Sin(temp * i * Mathf.Deg2Rad), 0);
            notes[i].SetDirection(direction);
            notes[i].transform.localPosition = Vector3.zero;
            notes[i].gameObject.SetActive(true);
        }
    }

    protected override IEnumerator Collapse()
    {
        foreach (BulletMove t in notes)
        {
            t.gameObject.SetActive(false);
        }

        return base.Collapse();
    }
}
