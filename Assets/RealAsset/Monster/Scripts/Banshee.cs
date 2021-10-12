using System.Collections;
using UnityEngine;

public class Banshee : NormalMonster
{
    private bool bRecognizeWakgood = false;
    [SerializeField] private BulletMove[] notes;

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
                spriteRenderer.flipX = transform.position.x < Wakgood.Instance.transform.position.x;
            }
            yield return ws01;
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(3f);

        WaitForSeconds ws5 = new(6f);
        while (true)
        {
            Cry();
            animator.SetTrigger("ATTACK");
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
        for (int i = 0; i < notes.Length; i++)
        {
            notes[i].gameObject.SetActive(false);
        }
        return base.Collapse();
    }
}
