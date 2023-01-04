using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class IcePanchi : NormalMonster
{

    [System.Serializable]
    private struct Note
    {
        public BulletMove[] notes;
        public DamagingObject[] noteDamagingObjects;
    }
    [SerializeField] private Note[] notes;

    private bool bRecognizeWakgood = false;
    private bool bIsAttacking = false;

    protected override void OnEnable()
    {
        bRecognizeWakgood = false;
        bIsAttacking = false;
        base.OnEnable();
        foreach (var _notes in notes)
        {
            for (int i = 0; i < _notes.notes.Length; i++)
            {
                _notes.notes[i].gameObject.SetActive(false);
                _notes.noteDamagingObjects[i].enabled = false;
            }
        }
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
        while (true)
        {
            bIsAttacking = true;
            Animator.SetTrigger("READY");

            yield return StartCoroutine(Casting(castingTime));
            Animator.SetTrigger("GO");

            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(Cry(i, (Vector3)Random.insideUnitCircle * 10f));
                yield return StartCoroutine(Casting(castingTime));
            }

            yield return new WaitForSeconds(3f);
            Animator.SetTrigger("OFF");
            bIsAttacking = false;

            yield return new WaitForSeconds(4f);
        }
    }

    private IEnumerator Cry(int noteIndex, Vector3 randomPos)
    {
        float theta = 360f / notes[noteIndex].notes.Length;

        for (int i = 0; i < notes[noteIndex].notes.Length; i++)
        {
            notes[noteIndex].notes[i].transform.localPosition = randomPos;
            notes[noteIndex].notes[i].transform.localRotation = Quaternion.Euler(i * theta * Vector3.forward);
            notes[noteIndex].notes[i].gameObject.SetActive(true);
            notes[noteIndex].notes[i].SetDirection(direction: new Vector3(Mathf.Cos((90 + theta * i) * Mathf.Deg2Rad), Mathf.Sin((90 + theta * i) * Mathf.Deg2Rad), 0));
            notes[noteIndex].notes[i].StopMove();
            notes[noteIndex].noteDamagingObjects[i].enabled = false;
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < notes[noteIndex].notes.Length; i++)
        {
            notes[noteIndex].notes[i].Move();
            notes[noteIndex].noteDamagingObjects[i].enabled = true;
        }
    }

    protected override void Collapse()
    {
        foreach (var _notes in notes)
            foreach (var note in _notes.notes)
                note.gameObject.SetActive(false);

        base.Collapse();
    }
}
