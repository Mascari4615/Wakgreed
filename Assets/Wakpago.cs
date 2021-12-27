using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wakpago : BossMonster
{
    [SerializeField] private float moveLimit = 15;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject monster1;
    [SerializeField] private GameObject monster2;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector3 spawnedPos = Vector3.zero;

    private int phase = 1;
    private Coroutine bulletCO;
    private Coroutine flipCO;
    private Coroutine bombCO;
    private Coroutine mobSpawnCO;
    private readonly BulletMove[] bullets = new BulletMove[50];
    private readonly TrailRenderer[] bulletsTR = new TrailRenderer[50];
    private readonly List<GameObject> monsterList = new();
    private readonly Collider2D[] bulletsCOL = new Collider2D[50];
    private bool bAttacking = false;
    private bool bCanUseMobSpawn = true;

    private readonly string[] nickNames = { "전투형 로봇", "마크 2", "최종 병기" };

    protected override void Awake()
    {
        base.Awake();

        nickName = nickNames[0];
        bAttacking = false;

        for (int i = 0; i < bullets.Length; i++)
        {
            GameObject m = Instantiate(bullet, transform);
            bullets[i] = m.GetComponent<BulletMove>();
            bulletsCOL[i] = m.GetComponent<Collider2D>();
            bulletsTR[i] = m.GetComponent<TrailRenderer>();
            m.SetActive(false);
        }

        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        spawnedPos = transform.position;
        phase = 1;
        bCanUseMobSpawn = true;
    }

    private void Initialize()
    {
        isCollapsed = false;
        hp = MaxHp;
        collider2D.enabled = true;
        Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        GameManager.Instance.enemyRunTimeSet.Add(gameObject);
        SpriteRenderer.sprite = defaultSprite;
    }

    protected override IEnumerator Attack()
    {
        flipCO = StartCoroutine(Flip());

        if (phase == 1)
        {
            while (true)
            {
                int a = bCanUseMobSpawn ? 1 : 0;
                int i = Random.Range(0, 1 + a + 1);

                switch (i)
                {
                    case 0:
                        yield return bulletCO = StartCoroutine(Bullet());
                        break;
                    case 1:
                        yield return mobSpawnCO =StartCoroutine(MobSpawn());
                        break;
                }
            }
        }
        else if (phase == 2)
        {
            while (true)
            {
                int a = bCanUseMobSpawn ? 1 : 0;
                int i = Random.Range(0, 1 + a + 1);

                switch (i)
                {
                    case 0:
                        yield return bulletCO = StartCoroutine(Bullet());
                        break;
                    case 1:
                        yield return mobSpawnCO = StartCoroutine(MobSpawn());

                        break;
                }
            }
        }
        else if (phase == 3)
        {
            bombCO = StartCoroutine(Bombs());

            while (true)
            {
                int a = bCanUseMobSpawn ? 1 : 0;
                int i = Random.Range(0, 1 + a + 1);

                switch (i)
                {
                    case 0:
                        yield return bulletCO = StartCoroutine(Bullet());
                        break;
                    case 1:
                        yield return mobSpawnCO = StartCoroutine(MobSpawn());

                        break;
                }
            }
        }

        yield break;
    }

    private IEnumerator Bombs()
    {
        while (true)
        {
            Vector3 randomPos = transform.position + (Vector3)Random.insideUnitCircle * 30f;
            ObjectManager.Instance.PopObject("Bomb", randomPos);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private IEnumerator Flip()
    {
        while (true)
        {
            if (bAttacking == false)
                SpriteRenderer.flipX = transform.position.x > Wakgood.Instance.transform.position.x;
            yield return null;
        }
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

        if (phase == 1)
        {
            monsterList.Add(ObjectManager.Instance.PopObject(monster1.name, a));
        }
        else if (phase == 2 || phase == 3)
            if (Random.Range(0, 1 + 1) == 0)
                monsterList.Add(ObjectManager.Instance.PopObject(monster1.name, a));
            else
                monsterList.Add(ObjectManager.Instance.PopObject(monster2.name, a));
    }

    private IEnumerator Bullet()
    {
        WaitForSeconds ws002 = new (.02f);
        bAttacking = true;
        for (int i = 0; i < bullets.Length; i++)
        {
            bulletsTR[i].Clear();
            bulletsCOL[i].enabled = false;
            bullets[i].enabled = false;
            bullets[i].gameObject.SetActive(false);
        }

        Animator.SetBool("SKILL1", true);

        Vector3 diff = Random.insideUnitCircle * 5f;
        Vector3 _attackDirection = (Wakgood.Instance.transform.position + diff - transform.position).normalized;

        lineRenderer.SetPosition(0, transform.position + (Vector3)Vector2.up);
        lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + _attackDirection * 100);
        lineRenderer.gameObject.SetActive(true);

        for (float i = 0; i <= 1f; i += Time.deltaTime * 0.5f)
        {
            diff = Vector3.Lerp(diff, Vector3.zero, i * 0.8f);
            _attackDirection = (Wakgood.Instance.transform.position + diff - transform.position).normalized;
            lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + _attackDirection * 100);
            yield return null;
        }

        Vector3 finalDirection = _attackDirection;
        lineRenderer.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].SetDirection(finalDirection + (Vector3)Random.insideUnitCircle * 0.3f);
            bulletsCOL[i].enabled = true;
            bullets[i].enabled = true;
            bullets[i].transform.position = transform.position + (Vector3)Vector2.up;
            bullets[i].gameObject.SetActive(true);

            yield return ws002;
        }

        yield return new WaitForSeconds(.7f);
        Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(1f);

        bAttacking = false;
    }

    protected override IEnumerator Collapse()
    {
        if (attackCO != null) StopCoroutine(attackCO);
        if (flipCO != null) StopCoroutine(flipCO);
        if (bulletCO != null) StopCoroutine(bulletCO);
        if (bombCO != null) StopCoroutine(bombCO);
        if (mobSpawnCO!= null) StopCoroutine(mobSpawnCO);
        lineRenderer.gameObject.SetActive(false);

        for (int i = 0; i < bullets.Length; i++)
        {
            bulletsTR[i].Clear();
            bulletsCOL[i].enabled = false;
            bullets[i].enabled = false;
            bullets[i].gameObject.SetActive(false);
        }

        SpriteRenderer.material = originalMaterial;

        isCollapsed = true;

        collider2D.enabled = false;
        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.bodyType = RigidbodyType2D.Static;
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        yield return StartCoroutine(UIManager.Instance.SpeedWagon_BossOff(this));

        onMonsterCollapse.Raise(transform);

        if (phase != 3)
        {
            phase++;
            Initialize();
            Animator.SetInteger("PHASE", phase);
            nickName = nickNames[phase - 1];
            yield return StartCoroutine(UIManager.Instance.SpeedWagon_BossOn(this));
            attackCO = StartCoroutine(Attack());
            yield break;
        }

        foreach (var monster in monsterList)
            ObjectManager.Instance.PushObject(monster);

        if (DataManager.Instance.CurGameData.killedOnceBoss[ID] == false)
        {
            if (Collection.Instance != null)
                Collection.Instance.Collect(this, true);
            DataManager.Instance.CurGameData.killedOnceBoss[ID] = true;
            DataManager.Instance.SaveGameData();
        }

        if (DataManager.Instance.CurGameData.rescuedNPC[npcID] == false)
        {
            DataManager.Instance.CurGameData.rescuedNPC[npcID] = true;
            DataManager.Instance.SaveGameData();
        }

        Animator.SetTrigger("COLLAPSE");
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);
        int randCount = Random.Range(0, 5);
        for (int i = 0; i < randCount; i++) ObjectManager.Instance.PopObject("ExpOrb", transform);
        ObjectManager.Instance.PopObject("LevelUpEffect", transform);
        (StageManager.Instance.CurrentRoom as BossRoom)?.RoomClear();
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
