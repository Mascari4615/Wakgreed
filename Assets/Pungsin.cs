using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pungsin : BossMonster
{
    [SerializeField] private GameObject ultAttackPrefab;
    [SerializeField] private GameObject skill1AttackPrefab;
    
    private BulletMove[] skill1AttackGo;
    [SerializeField] private GameObject ult;
    [SerializeField] private GameObject ultParticle1;
    [SerializeField] private GameObject ultParticle2;
    private GameObject[] ultAttackPos;
    private BulletMove[] ultAttackGos;
    [SerializeField] private GameObject stun;
    [SerializeField] private LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();
        // skill1 = transform.Find("Skill1").gameObject;
        ult = transform.Find("ult").gameObject;
        ultAttackPos = new GameObject[ult.transform.childCount];
        ultAttackGos = new BulletMove[ult.transform.childCount];
        for (int i = 0; i < ult.transform.childCount; i++)
        {
            ultAttackPos[i] = ult.transform.GetChild(i).gameObject;
            (ultAttackGos[i] = Instantiate(ultAttackPrefab, transform).GetComponent<BulletMove>()).gameObject.SetActive(false);
        }


        skill1AttackGo = new BulletMove[3];
        for (int i = 0; i < 3; i++)
        {
            (skill1AttackGo[i] = Instantiate(skill1AttackPrefab, transform).GetComponent<BulletMove>()).gameObject.SetActive(false);
        }

        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            int i = Random.Range(0, 1 + 1);
            switch (i)
            {
                case 0:
                    yield return StartCoroutine(Skill0());
                    break;
                case 1:
                    yield return StartCoroutine(Ult());
                    break;
            }
        }
    }

    private IEnumerator Ult()
    {
        Animator.SetBool("ULT", true);
        cinemachineTargetGroup.m_Targets[1].target = transform;

        float temp = 0;
        while (temp < .2f)
        {
            if (camera.m_Lens.OrthographicSize < 17) camera.m_Lens.OrthographicSize += 8 * Time.fixedDeltaTime;
            Wakgood.Instance.WakgoodMove.PlayerRb.AddForce((transform.position - Wakgood.Instance.transform.position).normalized * 600);
            yield return null;
            temp += Time.deltaTime;
        }

        ult.SetActive(true);
        ultParticle1.SetActive(true);

        temp = 0;
        while (temp < 4f)
        {
            if (camera.m_Lens.OrthographicSize > 12) camera.m_Lens.OrthographicSize -= 6 * Time.fixedDeltaTime;
            else if (camera.m_Lens.OrthographicSize > 10) camera.m_Lens.OrthographicSize -= 1 * Time.fixedDeltaTime;
            Wakgood.Instance.WakgoodMove.PlayerRb.AddForce((transform.position - Wakgood.Instance.transform.position).normalized * 600);
            yield return new WaitForFixedUpdate();
            temp += Time.fixedDeltaTime;
        }
        ultParticle1.SetActive(false);

        ultParticle2.SetActive(true);
        for (int i = 0; i < ultAttackGos.Length; i++)
        {
            ultAttackGos[i].transform.position = transform.position;
            ultAttackGos[i].SetDirection((ultAttackPos[i].transform.position - transform.position).normalized);
            ultAttackGos[i].gameObject.SetActive(true);
        }

        while (camera.m_Lens.OrthographicSize < 12)
        {
            camera.m_Lens.OrthographicSize += 5 * Time.deltaTime;
            yield return null;
        }

        ult.SetActive(false);
        Animator.SetBool("ULT", false);
        camera.m_Lens.OrthographicSize = 12;

        stun.SetActive(true);
        yield return new WaitForSeconds(6f);
        stun.SetActive(false);
        ultParticle2.SetActive(false);
    }

    private IEnumerator Skill0()
    {
        yield return new WaitForSeconds(.2f);
        Animator.SetBool("SKILL1", true);

        // int attackCount = Random.Range(2, 3 + 1);

        for (int i = 0; i < 3; i++)
        {
            Vector3 originPos = transform.position;
            Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3(
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f),
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

            Animator.SetTrigger("SKILL1CHARGE");

            SpriteRenderer.flipX = targetPos.x > Wakgood.Instance.transform.position.x;

            for (float j = 0; j <= 1; j += Time.deltaTime * 7)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return null;
            }

            Vector3 attackDirection = (Wakgood.Instance.transform.position - transform.position).normalized;

            lineRenderer.SetPosition(0, transform.position + (Vector3)Vector2.up);
            lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + attackDirection * 100);
            lineRenderer.gameObject.SetActive(true);

            yield return i == 0 ? new WaitForSeconds(.7f) : (object)new WaitForSeconds(.5f);

            Animator.SetTrigger("SKILL1GO");

            skill1AttackGo[i].transform.position = transform.position + (Vector3)Vector2.up;
            skill1AttackGo[i].SetDirection(attackDirection);
            skill1AttackGo[i].gameObject.SetActive(true);
            lineRenderer.gameObject.SetActive(false);

            yield return new WaitForSeconds(.2f);
        }

        Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(2f);

    }
}