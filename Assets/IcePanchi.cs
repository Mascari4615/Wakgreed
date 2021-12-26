using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePanchi : NormalMonster
{

    [System.Serializable]
    private struct Note
    {
        public BulletMove[] notes;
    }
    [SerializeField] private Note[] notes;

    private bool bRecognizeWakgood = false;
    private bool bIsAttacking = false;
    [SerializeField] private GameObject warning;

    private static readonly WaitForSeconds ws01 = new(0.1f);

    protected override void OnEnable()
    {
        bRecognizeWakgood = false;
        bIsAttacking = false;
        base.OnEnable();
        warning.SetActive(false);
        foreach (var _notes in notes)
            foreach (var note in _notes.notes)
                note.gameObject.SetActive(false);   
        StartCoroutine(CheckDistance());
    }

    private IEnumerator CheckDistance()
    {
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
                if (!bIsAttacking)
                    SpriteRenderer.flipX = transform.position.x < Wakgood.Instance.transform.position.x;
            yield return ws01;
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds ws5 = new(10f);

        while (true)
        {
            bIsAttacking = true;
            Animator.SetTrigger("READY");
            Vector3 randomPos = (Vector3)Random.insideUnitCircle * 10f;
            warning.transform.position = randomPos;
            warning.gameObject.SetActive(true);
            yield return StartCoroutine(Casting(1f));
            Animator.SetTrigger("GO");      
            Cry(0, randomPos);
            randomPos = (Vector3)Random.insideUnitCircle * 10f;
            warning.transform.position = randomPos;
            yield return StartCoroutine(Casting(1f));
            Cry(1, randomPos);
            randomPos = (Vector3)Random.insideUnitCircle * 10f;
            warning.transform.position = randomPos;
            yield return StartCoroutine(Casting(1f));
            Cry(2, randomPos);

            warning.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);
            Animator.SetTrigger("OFF");
            bIsAttacking = false;
            yield return ws5;
        }
    }

    private void Cry(int ang, Vector3 randomPos)
    {
        Vector3 direction = Vector3.zero;
        float temp = 360f / notes[ang].notes.Length;

        for (int i = 0; i < notes[ang].notes.Length; i++)
        {
            direction.Set(Mathf.Cos(temp * i * Mathf.Deg2Rad), Mathf.Sin(temp * i * Mathf.Deg2Rad), 0);
            notes[ang].notes[i].SetDirection(direction);
            notes[ang].notes[i].transform.localPosition = randomPos;
            notes[ang].notes[i].gameObject.SetActive(true);
        }
    }

    protected override IEnumerator Collapse()
    {
        foreach (var _notes in notes)
        {
            foreach (var note in _notes.notes)
            {
                note.gameObject.SetActive(false);
            }
        }

        return base.Collapse();
    }
}
