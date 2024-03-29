using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;

public class Hikiking : BossMonster
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float moveLimit = 15;
    [SerializeField] private GameObject stun;

    private Coroutine baseAttackCo;
    private Coroutine mobSpawnCo;
    private Coroutine mobSpawnCoCo;
    private Vector3 spawnedPos = Vector3.zero;
    private Coroutine ultCo;
    private int ultStack;

    protected override void Awake()
    {
        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));
        base.Awake();
    }

    protected override void OnEnable()
    {
        if (baseAttackCo != null)
        {
            StopCoroutine(baseAttackCo);
        }

        if (ultCo != null)
        {
            StopCoroutine(ultCo);
        }

        if (mobSpawnCo != null)
        {
            StopCoroutine(mobSpawnCo);
        }

        if (mobSpawnCoCo != null)
        {
            StopCoroutine(mobSpawnCoCo);
        }

        stun.SetActive(false);
        spawnedPos = transform.position;

        base.OnEnable();
    }

    protected override IEnumerator Attack()
    {
        mobSpawnCoCo = StartCoroutine(SpawnMobCo());
        while (true)
        {
            int i = Random.Range(0, 1 + 1);
            switch (i)
            {
                case 0:
                    yield return baseAttackCo = StartCoroutine(BaseAttack());
                    break;
                case 1:
                    yield return ultCo = StartCoroutine(Ult());
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }


    private IEnumerator BaseAttack()
    {
        int attackCount = Random.Range(3, 4 + 1);

        for (int i = 0; i < attackCount; i++)
        {
            Vector3 originPos = Rigidbody2D.transform.position;
            Vector3 targetPos = new(
                Mathf.Clamp(
                    Wakgood.Instance.transform.position.x + ((-1 + (Random.Range(0, 2) * 2)) * Random.Range(3f, 5f)),
                    spawnedPos.x - moveLimit, spawnedPos.x + moveLimit),
                Mathf.Clamp(
                    Wakgood.Instance.transform.position.y + ((-1 + (Random.Range(0, 2) * 2)) * Random.Range(3f, 5f)),
                    spawnedPos.y - moveLimit, spawnedPos.y + moveLimit));
            Animator.SetBool("BASE", true);
            for (float j = 0; j <= 1; j += 0.02f * 10)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return new WaitForSeconds(0.02f);
            }

            yield return new WaitForSeconds(0.2f);

            int slashCount = Random.Range(3, 6 + 1);
            for (int k = 0; k < slashCount; k++)
            {
                ObjectManager.Instance.PopObject("HikiSlash", Wakgood.Instance.transform.position,
                    new Vector3(0, 0, Random.Range(0f, 180f)));
                RuntimeManager.PlayOneShot("event:/SFX/Weapon/2");
                yield return new WaitForSeconds(.1f);
            }

            Animator.SetBool("BASE", false);
            yield return new WaitForSeconds(0.4f);
        }
    }

    private IEnumerator Ult()
    {
        cinemachineTargetGroup.m_Targets[1].target = transform;

        text.text = "공기가 요동칩니다...";
        text.gameObject.SetActive(true);
        Animator.SetBool("ULT", true);
        Animator.SetTrigger("READY");

        for (float j = 0; j <= 1; j += 3 * Time.fixedDeltaTime)
        {
            if (camera.m_Lens.OrthographicSize > 9)
            {
                camera.m_Lens.OrthographicSize -= 3 * Time.fixedDeltaTime;
            }

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
            targetPos[i] = new Vector3(
                Mathf.Clamp(
                    Wakgood.Instance.transform.position.x + (((i + 2) % 2 == 1 ? 1 : -1) * Random.Range(7f, 10f)),
                    spawnedPos.x - moveLimit, spawnedPos.x + moveLimit),
                Mathf.Clamp(
                    Wakgood.Instance.transform.position.y + ((-1 + (Random.Range(0, 2) * 2)) * Random.Range(7f, 10f)),
                    spawnedPos.y - moveLimit, spawnedPos.y + moveLimit));
        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.positionCount = ultCount + 1;
        lineRenderer.SetPosition(0, originPos);
        for (int i = 1; i < ultCount + 1; i++)
        {
            lineRenderer.SetPosition(i, targetPos[i - 1]);
        }

        lineRenderer.gameObject.SetActive(true);

        yield return new WaitForSeconds(.5f);

        Animator.SetTrigger("GO");
        text.gameObject.SetActive(false);

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

            GameObject temp = ObjectManager.Instance.PopObject("HikiSlash",
                originPos + ((targetPos[i] - originPos) / 2),
                new Vector3(0, 0,
                    Mathf.Rad2Deg * Mathf.Atan2(targetPos[i].y - originPos.y, targetPos[i].x - originPos.x)));
            temp.transform.localScale = new Vector3(Vector3.Distance(originPos, targetPos[i]) * 0.25f, 1.5f, 1);
            RuntimeManager.PlayOneShot("event:/SFX/Weapon/2");
        }

        collider2D.enabled = true;

        lineRenderer.gameObject.SetActive(false);
        lineRenderer.positionCount = 2;

        Vector3 aOriginPos = transform.position;
        Vector3 aTempPos = Wakgood.Instance.transform.position +
                           ((Wakgood.Instance.transform.position - transform.position).normalized * 50);

        Vector3 aTargetPos = new(
            Mathf.Clamp(aTempPos.x, spawnedPos.x - moveLimit, spawnedPos.x + moveLimit),
            Mathf.Clamp(aTempPos.y, spawnedPos.y - moveLimit, spawnedPos.y + moveLimit));

        lineRenderer.startWidth = 1f;

        lineRenderer.SetPosition(0, aOriginPos);
        lineRenderer.SetPosition(1, aTargetPos);
        lineRenderer.gameObject.SetActive(true);

        for (float j = 0; j <= 1f; j += 3.5f * Time.fixedDeltaTime)
        {
            if (camera.m_Lens.OrthographicSize > 6)
            {
                camera.m_Lens.OrthographicSize -= 2 * Time.fixedDeltaTime;
            }

            yield return new WaitForFixedUpdate();
        }

        collider2D.enabled = false;
        SpriteRenderer.flipX = transform.position.x > aTargetPos.x;

        for (float j = 0; Vector3.Distance(transform.position, aTargetPos) > .5f; j += Time.fixedDeltaTime * 30)
        {
            if (camera.m_Lens.OrthographicSize < 12)
            {
                camera.m_Lens.OrthographicSize += 5 * Time.fixedDeltaTime;
            }

            Rigidbody2D.transform.position = Vector3.Lerp(aOriginPos, aTargetPos, Mathf.Clamp01(j));
            yield return new WaitForFixedUpdate();
        }

        camera.m_Lens.OrthographicSize = 12;
        collider2D.enabled = true;

        GameObject v = ObjectManager.Instance.PopObject("HikiSlash2", aOriginPos + ((aTargetPos - aOriginPos) / 2),
            new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(aTargetPos.y - aOriginPos.y, aTargetPos.x - aOriginPos.x)));
        v.transform.localScale = new Vector3(Vector3.Distance(aOriginPos, aTargetPos) * 0.25f, 10, 1);
        RuntimeManager.PlayOneShot("event:/SFX/Weapon/2");
        yield return new WaitForSeconds(0.3f);
        lineRenderer.gameObject.SetActive(false);

        stun.SetActive(true);
        yield return new WaitForSeconds(4f);
        Animator.SetBool("ULT", false);
        stun.SetActive(false);
    }

    private IEnumerator SpawnMobCo()
    {
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                mobSpawnCo = StartCoroutine(SpawnMob());
            }

            yield return new WaitForSeconds(Random.Range(15f, 30f));
        }
    }

    private IEnumerator SpawnMob()
    {
        Vector3 pos = spawnedPos + ((Vector3)Random.insideUnitCircle * 10f);

        ObjectManager.Instance.PopObject("SpawnCircle", pos).GetComponent<Animator>().SetFloat("SPEED", 1 / 0.5f);
        yield return new WaitForSeconds(.5f);

        GameObject temp =
            ObjectManager.Instance.PopObject(Random.Range(0, 1 + 1) == 0 ? "ChidoriPanchi" : "SuriswordPanchi", pos);
        if (MobListContains(temp.GetInstanceID()))
        {
            monsterList.Add(temp);
        }
    }
}