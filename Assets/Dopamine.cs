using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dopamine : BossMonster
{
    [SerializeField] private float moveLimit = 1;
    [SerializeField] private GameObject wakpago;
    [SerializeField] private GameObject monkey;
    [SerializeField] private GameObject monster;
    [SerializeField] private LineRenderer[] lineRenderer;

    private readonly BulletMove[] monkeys = new BulletMove[3];
    private readonly Collider2D[] monkeysCOL = new Collider2D[3];
    private readonly TrailRenderer[] monkeysTR = new TrailRenderer[3];
    private readonly List<GameObject> monsterList = new();

    private Vector3 spawnedPos = Vector3.zero;
    private bool bCanUseMobSpawn = true;
    private bool bSpawnedWakpago = false;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < 3; i++)
        {
            GameObject m = Instantiate(monkey, transform);
            monkeys[i] = m.GetComponent<BulletMove>();
            monkeysCOL[i] = m.GetComponent<Collider2D>();
            monkeysTR[i] = m.GetComponent<TrailRenderer>();
            m.SetActive(false);
        }

        foreach (var lineRenderer in lineRenderer)
            lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        spawnedPos = transform.position;
        bCanUseMobSpawn = true;
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            int i = Random.Range(0, 0 + 1);

            if (bCanUseMobSpawn) i++;

            switch (i)
            {
                case 0:
                    yield return StartCoroutine(Monkey());
                    break;
                case 1:
                    yield return StartCoroutine(MobSpawn());
                    break;
            }
        }
    }

    private IEnumerator Monkey()
    {
        yield return new WaitForSeconds(.2f);
        Animator.SetBool("SKILL1", true);

        Vector3 originPos = transform.position;
        Vector3 targetPos = new(
            Mathf.Clamp(Wakgood.Instance.transform.position.x + (-1 + Random.Range(0, 2) * 2) * 6, spawnedPos.x - moveLimit, spawnedPos.x + moveLimit),
             Mathf.Clamp(Wakgood.Instance.transform.position.y + (-1 + Random.Range(0, 2) * 2) * 6, spawnedPos.y - moveLimit, spawnedPos.y + moveLimit));

        Animator.SetTrigger("SKILL1CHARGE");
        SpriteRenderer.flipX = targetPos.x > Wakgood.Instance.transform.position.x;

        for (float j = 0; j <= 1; j += Time.deltaTime * 7)
        {
            Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
            yield return null;
        }

        Vector3[] attackDirection = new Vector3[3];

        for (int i = 0; i < 3; i++)
        {
            attackDirection[i] = (Wakgood.Instance.transform.position - transform.position).normalized + (Vector3)Random.insideUnitCircle;

            lineRenderer[i].SetPosition(0, transform.position + (Vector3)Vector2.up);
            lineRenderer[i].SetPosition(1, transform.position + (Vector3)Vector2.up + attackDirection[i] * 100);
            lineRenderer[i].gameObject.SetActive(true);

            monkeysTR[i].Clear();
            monkeysCOL[i].enabled = false;
            monkeys[i].enabled = false;
            monkeys[i].transform.position = transform.position + (Vector3)Vector2.up + attackDirection[i];
            monkeys[i].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(.7f);

        Animator.SetTrigger("SKILL1GO");

        for (int i = 0; i < 3; i++)
        {
            lineRenderer[i].gameObject.SetActive(false);

            monkeys[i].SetDirection(attackDirection[i]);
            monkeys[i].enabled = true;
            monkeysCOL[i].enabled = true;
        }

        yield return new WaitForSeconds(.2f);
        Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator MobSpawn()
    {
        bCanUseMobSpawn = false;
        StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, 10f, value => bCanUseMobSpawn = value));

        if (monsterList.Count >= 6)
            yield break;

        for (int i = 0; i < 3; i++)
            StartCoroutine(SpawnMob());

        yield break;
    }

    private IEnumerator SpawnMob()
    {
        Vector3 pos = (Vector3)Random.insideUnitCircle * 10f;
        Vector3 a = new(
            Mathf.Clamp(transform.position.x + pos.x, spawnedPos.x - moveLimit, spawnedPos.x + moveLimit),
             Mathf.Clamp(transform.position.y + pos.y, spawnedPos.y - moveLimit, spawnedPos.y + moveLimit));

        ObjectManager.Instance.PopObject("SpawnCircle", a).GetComponent<Animator>().SetFloat("SPEED", 1 / 0.5f);
        yield return new WaitForSeconds(.5f);
        monsterList.Add(ObjectManager.Instance.PopObject(monster.name, a));
    }

    public override void ReceiveHit(int damage)
    {
        if (isCollapsed) return;

        if (bSpawnedWakpago == false && hp - damage < MaxHp * 0.7f)
        {
            StartCoroutine(SpawnWakpago());
        }

        base.ReceiveHit(damage);
    }

    private IEnumerator SpawnWakpago()
    {
        StopCoroutine(Attack());
        StopCoroutine(Monkey());

        foreach (var monkey in monkeys)
            monkey.gameObject.SetActive(false);

        bSpawnedWakpago = true;
        yield return new WaitForSeconds(1f);
        cinemachineTargetGroup.m_Targets[1].target = transform;
        // ¾Ö´Ï
        yield return new WaitForSeconds(3f);
        Instantiate(wakpago);
        yield return null;
    }

    protected override IEnumerator Collapse()
    {
        foreach (var monster in monsterList)
            ObjectManager.Instance.PushObject(monster);

        foreach (var monkey in monkeys)
            monkey.gameObject.SetActive(false);

        StartCoroutine(base.Collapse());
        yield return null;
    }
}
