using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wakmuse : NormalMonster
{
    [SerializeField] private Note[] notes;
    private Coroutine attack;

    private bool bIsAttacking;

    private Coroutine flip;

    protected override void OnEnable()
    {
        bIsAttacking = false;
        base.OnEnable();
        flip = StartCoroutine(Flip());
        attack = StartCoroutine(Attack());
    }

    private IEnumerator Flip()
    {
        while (true)
        {
            if (!bIsAttacking)
            {
                SpriteRenderer.flipX = transform.position.x < Wakgood.Instance.transform.position.x;
            }

            yield return ws01;
        }
    }


    private IEnumerator Attack()
    {
        WaitForSeconds ws3 = new(3f);
        yield return ws1;
        while (true)
        {
            bIsAttacking = true;
            Animator.SetTrigger("READY");
            yield return StartCoroutine(Casting(castingTime));
            Animator.SetTrigger("GO");
            Cry();
            bIsAttacking = false;

            yield return ws3;
        }
    }

    private void Cry()
    {
        Vector3 direction = Vector3.zero;
        int random = Random.Range(0, notes.Length);

        float temp = 360f / notes[random].notes.Length;


        for (int i = 0; i < notes[random].notes.Length; i++)
        {
            direction.Set(Mathf.Cos(temp * i * Mathf.Deg2Rad), Mathf.Sin(temp * i * Mathf.Deg2Rad), 0);
            notes[random].notes[i].SetDirection(direction);
            notes[random].notes[i].transform.localPosition = Vector3.zero;
            notes[random].notes[i].gameObject.SetActive(true);
        }
    }

    protected override void Collapse()
    {
        if (attack != null)
        {
            StopCoroutine(attack);
        }

        if (flip != null)
        {
            StopCoroutine(flip);
        }

        foreach (Note _notes in notes)
        {
            foreach (BulletMove note in _notes.notes)
            {
                note.gameObject.SetActive(false);
            }
        }

        base.Collapse();
    }

    [Serializable]
    private struct Note
    {
        public BulletMove[] notes;
    }
}