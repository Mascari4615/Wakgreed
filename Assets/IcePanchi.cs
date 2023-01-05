using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePanchi : NormalMonster
{
    private const int ICE_MOONGTANGE_COUNT = 3;
    private const int ICE_PIECE_COUNT = 3;

    private class IceMoongtange
    {
        public BulletMove[] ices = new BulletMove[ICE_PIECE_COUNT];
        public Collider2D[] iceColliders = new Collider2D[ICE_PIECE_COUNT];
    }
    private IceMoongtange[] IceMoongtanges = new IceMoongtange[ICE_MOONGTANGE_COUNT];

    private bool bRecognizeWakgood = false;
    private bool bIsAttacking = false;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < ICE_MOONGTANGE_COUNT; i++)
            IceMoongtanges[i] = new IceMoongtange();

        Transform iceMoongtanges = transform.Find("Ices");
        for (int i = 0; i < iceMoongtanges.childCount; i++)
        {
            Transform ice = iceMoongtanges.GetChild(i);
            IceMoongtanges[i / ICE_PIECE_COUNT].ices[i % ICE_PIECE_COUNT] = ice.GetComponent<BulletMove>();
            IceMoongtanges[i / ICE_PIECE_COUNT].iceColliders[i % ICE_PIECE_COUNT] = ice.GetComponent<Collider2D>();
        }
    }

    protected override void OnEnable()
    {
        bRecognizeWakgood = false;
        bIsAttacking = false;
        base.OnEnable();
        foreach (var iceMoongtange in IceMoongtanges)
        {
            for (int i = 0; i < ICE_PIECE_COUNT; i++)
            {
                iceMoongtange.ices[i].gameObject.SetActive(false);
                iceMoongtange.iceColliders[i].enabled = false;
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

            for (int i = 0; i < ICE_MOONGTANGE_COUNT; i++)
            {
                StartCoroutine(SpawnIceMoongtange(i, (Vector3)Random.insideUnitCircle * 10f));

                if (i != ICE_MOONGTANGE_COUNT - 1)
                    yield return StartCoroutine(Casting(castingTime));
            }

            yield return new WaitForSeconds(3f);
            Animator.SetTrigger("OFF");
            bIsAttacking = false;

            yield return new WaitForSeconds(4f);
        }
    }

    private IEnumerator SpawnIceMoongtange(int moongtangeIndex, Vector3 spawnPos)
    {
        float theta = 360f / ICE_PIECE_COUNT;

        for (int i = 0; i < ICE_PIECE_COUNT; i++)
        {
            IceMoongtanges[moongtangeIndex].ices[i].transform.localPosition = spawnPos;
            IceMoongtanges[moongtangeIndex].ices[i].transform.localRotation = Quaternion.Euler(i * theta * Vector3.forward);
            IceMoongtanges[moongtangeIndex].ices[i].gameObject.SetActive(true);
            IceMoongtanges[moongtangeIndex].ices[i].SetDirection
                (direction: new Vector3(Mathf.Cos((90 + theta * i) * Mathf.Deg2Rad), Mathf.Sin((90 + theta * i) * Mathf.Deg2Rad), 0));
            IceMoongtanges[moongtangeIndex].ices[i].StopMove();
            IceMoongtanges[moongtangeIndex].iceColliders[i].enabled = false;
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < ICE_PIECE_COUNT; i++)
        {
            IceMoongtanges[moongtangeIndex].ices[i].Move();
            IceMoongtanges[moongtangeIndex].iceColliders[i].enabled = true;
        }
    }

    protected override void Collapse()
    {
        foreach (var iceMoongtange in IceMoongtanges)
            foreach (var ice in iceMoongtange.ices)
                ice.gameObject.SetActive(false);

        base.Collapse();
    }
}
