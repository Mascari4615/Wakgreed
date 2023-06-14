using System.Collections;
using UnityEngine;

public class Pungsin : BossMonster
{
    [SerializeField] private BulletMove[] ultGO;
    [SerializeField] private BulletMove[] skill0BulletGo;
    [SerializeField] private BulletMoveStar[] skill1BulletGo;

    [SerializeField] private GameObject ultWarning;
    [SerializeField] private GameObject ultReadyParticle;
    [SerializeField] private GameObject ultGoParticle;

    [SerializeField] private GameObject stun;
    [SerializeField] private LineRenderer skill1Warning;
    [SerializeField] private float moveLimit;
    private Coroutine skill0Co;
    private Coroutine skill1Co;
    private Vector3 spawnedPos = Vector3.zero;
    private int temp;
    private Coroutine ultCo;

    protected override void Awake()
    {
        base.Awake();
        skill1Warning.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));
    }

    protected override void OnEnable()
    {
        if (skill1Co != null)
        {
            StopCoroutine(skill1Co);
        }

        if (ultCo != null)
        {
            StopCoroutine(ultCo);
        }

        if (skill0Co != null)
        {
            StopCoroutine(skill0Co);
        }

        foreach (BulletMove item in ultGO)
        {
            item.gameObject.SetActive(false);
        }

        foreach (BulletMove item in skill0BulletGo)
        {
            item.gameObject.SetActive(false);
        }

        foreach (BulletMoveStar item in skill1BulletGo)
        {
            item.gameObject.SetActive(false);
        }

        ultWarning.SetActive(false);
        ultGoParticle.SetActive(false);
        ultReadyParticle.SetActive(false);
        stun.gameObject.SetActive(false);

        spawnedPos = transform.position;
        temp = 0;

        Animator.SetBool("ULT", false);
        Animator.SetBool("SKILL1", false);

        base.OnEnable();
    }

    protected override IEnumerator _Collapse()
    {
        if (skill1Co != null)
        {
            StopCoroutine(skill1Co);
        }

        if (ultCo != null)
        {
            StopCoroutine(ultCo);
        }

        if (skill0Co != null)
        {
            StopCoroutine(skill0Co);
        }

        foreach (BulletMove item in ultGO)
        {
            item.gameObject.SetActive(false);
        }

        foreach (BulletMove item in skill0BulletGo)
        {
            item.gameObject.SetActive(false);
        }

        foreach (BulletMoveStar item in skill1BulletGo)
        {
            item.gameObject.SetActive(false);
        }

        ultWarning.SetActive(false);
        ultGoParticle.SetActive(false);
        ultReadyParticle.SetActive(false);
        stun.gameObject.SetActive(false);

        return base._Collapse();
    }

    protected override IEnumerator Attack()
    {
        yield return StartCoroutine(Skill1());

        while (true)
        {
            int a = temp < skill1BulletGo.Length / 2 ? 1 : 0;
            int i = Random.Range(0, 3 + a);

            switch (i)
            {
                case 0:
                case 1:
                    yield return skill1Co = StartCoroutine(Skill0());
                    break;
                case 2:
                    yield return ultCo = StartCoroutine(Ult());
                    break;
                case 3:
                    yield return skill1Co = StartCoroutine(Skill1());
                    break;
            }
        }
    }

    private IEnumerator Skill1()
    {
        Animator.SetBool("ULT", true);
        yield return new WaitForSeconds(1);
        skill1BulletGo[temp * 2].Set(Random.Range(.2f, .5f), Random.Range(2.5f, 5f));
        skill1BulletGo[temp * 2].gameObject.SetActive(true);
        skill1BulletGo[(temp * 2) + 1].Set(Random.Range(.2f, .5f), Random.Range(2.5f, 5f));
        skill1BulletGo[(temp * 2) + 1].gameObject.SetActive(true);
        Animator.SetBool("ULT", false);
        temp++;
        yield return new WaitForSeconds(2);
    }

    private IEnumerator Ult()
    {
        Animator.SetBool("ULT", true);
        cinemachineTargetGroup.m_Targets[1].target = transform;

        float temp = 0;
        while (temp < .2f)
        {
            if (camera.m_Lens.OrthographicSize < 17)
            {
                camera.m_Lens.OrthographicSize += 8 * Time.fixedDeltaTime;
            }

            Wakgood.Instance.WakgoodMove.PlayerRb.AddForce((transform.position - Wakgood.Instance.transform.position)
                .normalized * 600);
            yield return null;
            temp += Time.deltaTime;
        }

        ultWarning.SetActive(true);
        ultReadyParticle.SetActive(true);

        temp = 0;
        while (temp < 4f)
        {
            if (camera.m_Lens.OrthographicSize > 12)
            {
                camera.m_Lens.OrthographicSize -= 6 * Time.fixedDeltaTime;
            }
            else if (camera.m_Lens.OrthographicSize > 10)
            {
                camera.m_Lens.OrthographicSize -= 1 * Time.fixedDeltaTime;
            }

            Wakgood.Instance.WakgoodMove.PlayerRb.AddForce((transform.position - Wakgood.Instance.transform.position)
                .normalized * 600);
            yield return new WaitForFixedUpdate();
            temp += Time.fixedDeltaTime;
        }

        ultReadyParticle.SetActive(false);

        ultGoParticle.SetActive(true);
        for (int i = 0; i < ultGO.Length; i++)
        {
            ultGO[i].transform.position = transform.position;
            ultGO[i].SetDirection((ultWarning.transform.GetChild(i).position - transform.position).normalized);
            ultGO[i].gameObject.SetActive(true);
        }

        while (camera.m_Lens.OrthographicSize < 12)
        {
            camera.m_Lens.OrthographicSize += 5 * Time.deltaTime;
            yield return null;
        }

        ultWarning.SetActive(false);
        Animator.SetBool("ULT", false);
        camera.m_Lens.OrthographicSize = 12;

        stun.SetActive(true);
        yield return new WaitForSeconds(6f);
        stun.SetActive(false);
        ultGoParticle.SetActive(false);
    }

    private IEnumerator Skill0()
    {
        yield return new WaitForSeconds(.2f);
        Animator.SetBool("SKILL1", true);

        for (int i = 0; i < 3; i++)
        {
            Vector3 originPos = transform.position;
            Vector3 targetPos = new(
                Mathf.Clamp(
                    Wakgood.Instance.transform.position.x + ((-1 + (Random.Range(0, 2) * 2)) * Random.Range(3f, 5f)),
                    spawnedPos.x - moveLimit, spawnedPos.x + moveLimit),
                Mathf.Clamp(
                    Wakgood.Instance.transform.position.y + ((-1 + (Random.Range(0, 2) * 2)) * Random.Range(3f, 5f)),
                    spawnedPos.y - moveLimit, spawnedPos.y + moveLimit));

            Animator.SetTrigger("SKILL1CHARGE");

            SpriteRenderer.flipX = targetPos.x > Wakgood.Instance.transform.position.x;

            for (float j = 0; j <= 1; j += Time.deltaTime * 7)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return null;
            }

            Vector3 attackDirection = (Wakgood.Instance.transform.position - transform.position).normalized;

            skill1Warning.SetPosition(0, transform.position + (Vector3)Vector2.up);
            skill1Warning.SetPosition(1, transform.position + (Vector3)Vector2.up + (attackDirection * 100));
            skill1Warning.gameObject.SetActive(true);

            yield return i == 0 ? new WaitForSeconds(.7f) : new WaitForSeconds(.4f);

            Animator.SetTrigger("SKILL1GO");

            skill0BulletGo[i].transform.position = transform.position + (Vector3)Vector2.up;
            skill0BulletGo[i].SetDirection(attackDirection);
            skill0BulletGo[i].gameObject.SetActive(true);
            skill1Warning.gameObject.SetActive(false);

            yield return new WaitForSeconds(.2f);
        }

        Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(2f);
    }
}