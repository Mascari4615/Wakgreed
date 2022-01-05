using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public abstract class BossMonster : Monster
{
    public int npcID;
    public string nickName;
    protected CinemachineTargetGroup cinemachineTargetGroup;
    protected new CinemachineVirtualCamera camera;
    protected Coroutine attackCO;

    [SerializeField] private bool TESTING = false;

    protected override void Awake()
    {
        base.Awake();
        camera = GameObject.Find("Cameras").transform.Find("CM Camera").GetComponent<CinemachineVirtualCamera>();
        cinemachineTargetGroup = GameObject.Find("Cameras").transform.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
    }

    protected override void OnEnable()
    {
        isCollapsed = false;
        hp = MaxHp;
        collider2D.enabled = true;
        Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        SpriteRenderer.sprite = defaultSprite;
        Animator.SetTrigger("AWAKE");

        if (TESTING)
        {
            StartCoroutine(Ready());
        }

        if (StageManager.Instance.CurrentRoom != null)
        {
            if (StageManager.Instance.CurrentRoom is BossRoom)
            {
                StartCoroutine(Ready());
            }
        }

        GameManager.Instance.enemyRunTimeSet.Add(gameObject);
    }

    private IEnumerator Ready()
    {
        yield return StartCoroutine(UIManager.Instance.SpeedWagon_BossOn(this));
        yield return new WaitForSeconds(2f);
        attackCO = StartCoroutine(nameof(Attack));
    }

    protected List<GameObject> monsterList = new();

    protected abstract IEnumerator Attack();
    
    protected override void Collapse()
    {
        StartCoroutine(_Collapse());
    }

    protected bool MobListContains(int mobInstanceID)
    {
        foreach (var mob in monsterList)
        {
            if (mobInstanceID.Equals(mob.GetInstanceID()))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual IEnumerator _Collapse()
    {
        isCollapsed = true;

        if (DataManager.Instance.CurGameData.rescuedNPC[npcID] == false)
        {
            DataManager.Instance.CurGameData.rescuedNPC[npcID] = true;
            DataManager.Instance.SaveGameData();
        }

        SpriteRenderer.material = originalMaterial;

        cinemachineTargetGroup.m_Targets[1].target = null;
        RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", System.StringComparison.Ordinal), 7) : name)}_Collapse", transform.position);

        collider2D.enabled = false;
        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.bodyType = RigidbodyType2D.Static;
        Animator.SetTrigger("COLLAPSE");
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);
        onMonsterCollapse.Raise(transform);
        int randCount = Random.Range(0, 5);
        for (int i = 0; i < randCount; i++) ObjectManager.Instance.PopObject("ExpOrb", transform);

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        foreach (var monster in monsterList)
        {
            if (monster.activeSelf)
            {
                ObjectManager.Instance.PushObject(monster);
            }
        }

        yield return StartCoroutine(UIManager.Instance.SpeedWagon_BossOff(this));
        yield return new WaitForSeconds(3f);

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        (StageManager.Instance.CurrentRoom as BossRoom)?.RoomClear();
        gameObject.SetActive(false);
    }

    protected override void OnDisable()
    {
        if (attackCO != null) StopCoroutine(attackCO);
        base.OnDisable();
    }
}
