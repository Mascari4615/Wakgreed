using System.Collections;
using UnityEngine;

public class Pungsin : BossMonster
{
    [SerializeField] private GameObject ultAttackPrefab;
    [SerializeField] private GameObject skill0AttackPrefab;
    [SerializeField] private GameObject skill1AttackPrefab;

    private BulletMove[] skill0AttackGo;
    private BulletMoveStar[] skill1AttackGo;
    [SerializeField] private GameObject ult;
    [SerializeField] private GameObject ultParticle1;
    [SerializeField] private GameObject ultParticle2;
    private GameObject[] ultAttackPos;
    private BulletMove[] ultAttackGos;
    [SerializeField] private GameObject stun;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector3 spawnedPos = Vector3.zero;
    [SerializeField] private float moveLimit = 15;

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

        skill0AttackGo = new BulletMove[3];
        for (int i = 0; i < 3; i++)
        {
            (skill0AttackGo[i] = Instantiate(skill0AttackPrefab, transform).GetComponent<BulletMove>()).gameObject.SetActive(false);
        }

        skill1AttackGo = new BulletMoveStar[6];
        for (int i = 0; i < skill1AttackGo.Length; i++)
            (skill1AttackGo[i] = Instantiate(skill1AttackPrefab, transform).GetComponent<BulletMoveStar>()).gameObject.SetActive(false);

        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));
    }
    int temp = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        spawnedPos = transform.position;
    }

    protected override IEnumerator _Collapse()
    {
        foreach (var item in skill0AttackGo)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in skill1AttackGo)
        {
            item.gameObject.SetActive(false);
        }

        return base._Collapse();
    }

    protected override IEnumerator Attack()
    {
        yield return StartCoroutine(Skill1());

        while (true)
        {
            int a = temp < skill1AttackGo.Length / 2 ? 1 : 0;
            int i = Random.Range(0, 3 + a);

            switch (i)
            {
                case 0:
                case 1:
                    yield return StartCoroutine(Skill0());
                    break;
                case 2:
                    yield return StartCoroutine(Ult());
                    break;
                case 3:
                    yield return StartCoroutine(Skill1());
                    break;
            }
        }
    }

    private IEnumerator Skill1()
    {
        Animator.SetBool("ULT", true);
        yield return new WaitForSeconds(1);
        skill1AttackGo[temp * 2].Set(Random.Range(.2f, .5f), Random.Range(2.5f, 5f));
        skill1AttackGo[temp * 2].gameObject.SetActive(true);
        skill1AttackGo[temp * 2 + 1].Set(Random.Range(.2f, .5f), Random.Range(2.5f, 5f));
        skill1AttackGo[temp * 2 + 1].gameObject.SetActive(true);
        Animator.SetBool("ULT", false);
        temp++;
        yield return new WaitForSeconds(2);
        yield break;
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

        for (int i = 0; i < 3; i++)
        {
            Vector3 originPos = transform.position;
            Vector3 targetPos = new(
Mathf.Clamp(Wakgood.Instance.transform.position.x + (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f), spawnedPos.x - moveLimit, spawnedPos.x + moveLimit),
Mathf.Clamp(Wakgood.Instance.transform.position.y + (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f), spawnedPos.y - moveLimit, spawnedPos.y + moveLimit));

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

            yield return i == 0 ? new WaitForSeconds(.7f) : new WaitForSeconds(.4f);

            Animator.SetTrigger("SKILL1GO");

            skill0AttackGo[i].transform.position = transform.position + (Vector3)Vector2.up;
            skill0AttackGo[i].SetDirection(attackDirection);
            skill0AttackGo[i].gameObject.SetActive(true);
            lineRenderer.gameObject.SetActive(false);

            yield return new WaitForSeconds(.2f);
        }

        Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(2f);

    }
}