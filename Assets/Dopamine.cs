using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dopamine : BossMonster
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject ultAttack1;
    [SerializeField] private GameObject ultAttack2;
    [SerializeField] private GameObject monster;
    private int ultStack = 0;

    protected override IEnumerator Attack()
    {
        while (true)
        {
            int i = Random.Range(0, 3 + 1);
            switch (i)
            {
                case 0:
                    yield return StartCoroutine(BaseAttack());
                    break;
                case 1:
                    yield return StartCoroutine(Ult());
                    break;
                case 2:
                    yield return StartCoroutine(Skill1());
                    break;
                case 3:
                    yield return StartCoroutine(Skill2());
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator BaseAttack()
    {
        int attackCount = Random.Range(2, 3 + 1);

        for (int i = 0; i < attackCount; i++)
        {
            Vector3 originPos = Rigidbody2D.transform.position;
            Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3(
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f),
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

            for (float j = 0; j <= 1; j += 0.02f * 10)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return new WaitForSeconds(0.02f);
            }

            yield return new WaitForSeconds(0.2f);

            int slashCount = Random.Range(2, 4 + 1);
            for (int k = 0; k < slashCount; k++)
            {
                Instantiate(ultAttack1, Wakgood.Instance.transform.position,
                    Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 180f))));
                yield return new WaitForSeconds(.1f);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator Ult()
    {
        cinemachineTargetGroup.m_Targets[1].target = transform;

        Animator.SetTrigger("READY");

        for (float j = 0; j <= 1; j += 3 * Time.fixedDeltaTime)
        {
            if (camera.m_Lens.OrthographicSize > 9) camera.m_Lens.OrthographicSize -= 3 * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        ultStack++;
        int ultCount = ultStack switch
        {
            1 => 0,
            2 => 2,
            _ => 5
        };

        Vector3 originPos = Rigidbody2D.transform.position;
        Vector3[] targetPos = new Vector3[ultCount];

        for (int i = 0; i < ultCount; i++)
        {
            targetPos[i] = Wakgood.Instance.transform.position + new Vector3(((i + 2) % 2 == 1 ? 1 : -1) * Random.Range(7f, 10f), (-1 + Random.Range(0, 2) * 2) * Random.Range(7f, 10f));
        }

        lineRenderer.positionCount = ultCount + 1;
        lineRenderer.SetPosition(0, originPos);
        for (int i = 1; i < ultCount + 1; i++)
            lineRenderer.SetPosition(i, targetPos[i - 1]);
        lineRenderer.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        Animator.SetTrigger("GO");

        collider2D.enabled = false;
        for (int i = 0; i < ultCount; i++)
        {
            SpriteRenderer.flipX = transform.position.x > targetPos[i].x;

            for (float j = 0; Vector3.Distance(transform.position, targetPos[i]) > .5f; j += Time.fixedDeltaTime * 15)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos[i], j);
                yield return new WaitForFixedUpdate();
            }

            originPos = targetPos[i];

            var temp = Instantiate(ultAttack1, originPos + (targetPos[i] - originPos) / 2, Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(targetPos[i].y - originPos.y, targetPos[i].x - originPos.x))));
            temp.transform.localScale = new Vector3(Vector3.Distance(originPos, targetPos[i]) * 0.25f, 1, 1);
        }
        collider2D.enabled = true;

        lineRenderer.gameObject.SetActive(false);
        lineRenderer.positionCount = 2;

        Vector3 aOriginPos = transform.position;
        Vector3 aTargetPos = Wakgood.Instance.transform.position +
                             (Wakgood.Instance.transform.position - transform.position).normalized * 20;

        lineRenderer.SetPosition(0, aOriginPos);
        lineRenderer.SetPosition(1, aTargetPos);
        lineRenderer.gameObject.SetActive(true);

        for (float j = 0; j <= 1f; j += 2 * Time.fixedDeltaTime)
        {
            if (camera.m_Lens.OrthographicSize > 6) camera.m_Lens.OrthographicSize -= 2 * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        collider2D.enabled = false;
        SpriteRenderer.flipX = transform.position.x > aTargetPos.x;

        for (float j = 0; Vector3.Distance(transform.position, aTargetPos) > .5f; j += Time.fixedDeltaTime * 30)
        {
            if (camera.m_Lens.OrthographicSize < 12) camera.m_Lens.OrthographicSize += 5 * Time.fixedDeltaTime;
            Rigidbody2D.transform.position = Vector3.Lerp(aOriginPos, aTargetPos, j);
            yield return new WaitForFixedUpdate();
        }
        camera.m_Lens.OrthographicSize = 12;
        collider2D.enabled = true;

        var v = Instantiate(ultAttack2, aOriginPos + (aTargetPos - aOriginPos) / 2, Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(aTargetPos.y - aOriginPos.y, aTargetPos.x - aOriginPos.x))));
        v.transform.localScale = new Vector3(Vector3.Distance(aOriginPos, aTargetPos) * 0.25f, 8, 1);

        yield return new WaitForSeconds(0.3f);
        lineRenderer.gameObject.SetActive(false);
        Animator.SetTrigger("IDLE");

        yield return new WaitForSeconds(3f);
    }

    private IEnumerator Skill1()
    {
        int jjabSlayerCount = Random.Range(2, 5 + 1);
        for (int i = 0; i < jjabSlayerCount; i++)
            StartCoroutine(SpawnMob());

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator SpawnMob()
    {
        Vector3 randomPos = transform.position + (Vector3)Random.insideUnitCircle * 10f;
        ObjectManager.Instance.PopObject("SpawnCircle", randomPos).GetComponent<Animator>().SetFloat("SPEED", 1 / 0.5f);
        yield return new WaitForSeconds(.5f);
        ObjectManager.Instance.PopObject(monster.name, randomPos);
    }

    private IEnumerator Skill2()
    {
        /*int jjabSlayerCount = Random.Range(2, 5 + 1);
        for (int i = 0; i < jjabSlayerCount; i++)
            StartCoroutine(SpawnMob());*/

        yield return new WaitForSeconds(0.5f);
    }
}
