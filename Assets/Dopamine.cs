using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using TMPro;

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

    private Vector3 spawnedPos = Vector3.zero;
    private bool bCanUseMobSpawn = true;
    private Coroutine monkeyCO;
    private Coroutine mobSpawnCO;
    private Coroutine mobSpawnCOCO;

    [TextArea] [SerializeField] private List<string> comment;
    private GameObject chat;
    private TextMeshProUGUI chatText;

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

        chat = transform.GetChild(0).transform.Find("Chat").gameObject;
        chatText = chat.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    protected override void OnEnable()
    {
        if (monkeyCO != null) StopCoroutine(monkeyCO);
        if (mobSpawnCO != null) StopCoroutine(mobSpawnCO);
        if (mobSpawnCOCO != null) StopCoroutine(mobSpawnCOCO);
        spawnedPos = transform.position;
        bCanUseMobSpawn = true;
        base.OnEnable();
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            int a = bCanUseMobSpawn ? 1 : 0;
            int i = Random.Range(0, 0 + a + 1);

            switch (i)
            {
                case 0:
                    yield return monkeyCO = StartCoroutine(Monkey());
                    break;
                case 1:
                    yield return mobSpawnCOCO = StartCoroutine(MobSpawn());
                    break;
            }
        }
    }

    private IEnumerator Monkey()
    {
        yield return new WaitForSeconds(.2f);
        // Animator.SetBool("SKILL1", true);

        Vector3 originPos = transform.position;
        Vector3 targetPos = new(
            Mathf.Clamp(Wakgood.Instance.transform.position.x + (-1 + Random.Range(0, 2) * 2) * 6, spawnedPos.x - moveLimit, spawnedPos.x + moveLimit),
             Mathf.Clamp(Wakgood.Instance.transform.position.y + (-1 + Random.Range(0, 2) * 2) * 6, spawnedPos.y - moveLimit, spawnedPos.y + moveLimit));

        // Animator.SetTrigger("SKILL1CHARGE");
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

        // Animator.SetTrigger("SKILL1GO");

        for (int i = 0; i < 3; i++)
        {
            lineRenderer[i].gameObject.SetActive(false);

            monkeys[i].SetDirection(attackDirection[i]);
            monkeys[i].enabled = true;
            monkeysCOL[i].enabled = true;
        }

        yield return new WaitForSeconds(.2f);
        // Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator MobSpawn()
    {
        bCanUseMobSpawn = false;
        StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, 10f, value => bCanUseMobSpawn = value));

        if (monsterList.Count >= 6)
            yield break;

        for (int i = 0; i < 3; i++)
            mobSpawnCO = StartCoroutine(SpawnMob());

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
        GameObject temp = ObjectManager.Instance.PopObject(monster.name, a);
        if (MobListContains(temp.GetInstanceID()))
        {
            monsterList.Add(temp);
        }
    }

    private IEnumerator SpawnWakpago()
    {
        if (attackCO != null) StopCoroutine(attackCO);
        if (monkeyCO != null) StopCoroutine(monkeyCO);

        foreach (var monkey in monkeys)
            monkey.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        // ¾Ö´Ï
        yield return StartCoroutine(OnType());
        ObjectManager.Instance.PopObject(wakpago.name, spawnedPos);
    }

    protected override IEnumerator _Collapse()
    {
        SpriteRenderer.material = originalMaterial;

        isCollapsed = true;

        if (DataManager.Instance.CurGameData.rescuedNPC[npcID] == false)
        {
            DataManager.Instance.CurGameData.rescuedNPC[npcID] = true;
            DataManager.Instance.SaveGameData();
        }

        collider2D.enabled = false;
        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.bodyType = RigidbodyType2D.Static;
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);
        onMonsterCollapse.Raise(transform);

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        foreach (var monster in monsterList)
        {
            if (monster.activeSelf)
            {
                ObjectManager.Instance.PushObject(monster);
            }
        }

        foreach (var lineRenderer in lineRenderer)
            lineRenderer.gameObject.SetActive(false);

        yield return StartCoroutine(UIManager.Instance.SpeedWagon_BossOff(this));
        yield return StartCoroutine(SpawnWakpago());

        foreach (var monkey in monkeys)
            monkey.gameObject.SetActive(false);        

        // Animator.SetTrigger("COLLAPSE");
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private IEnumerator OnType()
    {
        WaitForSeconds ws005 = new(0.05f);

        cinemachineTargetGroup.m_Targets[0].target = transform;
        cinemachineTargetGroup.m_Targets[1].target = Wakgood.Instance.transform;

        chat.SetActive(true);
        foreach (string t in comment)
        {
            chatText.text = "";

            foreach (char item in t)
            {
                chatText.text += item;
                RuntimeManager.PlayOneShot($"event:/SFX/NPC/NPC_{npcID}", transform.position);
                yield return ws005;
            }

            yield return new WaitForSeconds(.5f);
        }
        chat.SetActive(false);

        yield return new WaitForSeconds(1f);

        cinemachineTargetGroup.m_Targets[0].target = Wakgood.Instance.transform;
        cinemachineTargetGroup.m_Targets[1].target = null;
    }
}
