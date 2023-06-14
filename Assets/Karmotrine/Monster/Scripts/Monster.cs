using FMODUnity;
using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IHitable
{
    protected static WaitForSeconds ws01 = new(0.1f);
    protected static WaitForSeconds ws1 = new(1);
    [SerializeField] private Material flashMaterial;
    [SerializeField] protected GameEvent onMonsterCollapse;
    public string mobName;
    public int ID;
    [TextArea] public string description;
    public int hp;
    [SerializeField] protected float MoveSpeed;
    public Sprite defaultSprite;
    protected Animator Animator;
    private string collapseSFX;
    protected new Collider2D collider2D;
    private Coroutine flashRoutine;
    private string hurtSFX;
    protected bool isCollapsed;
    protected Material originalMaterial;
    protected Rigidbody2D Rigidbody2D;
    protected SpriteRenderer SpriteRenderer;
    public int MaxHp { get; protected set; }

    protected virtual void Awake()
    {
        MaxHp = hp;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        originalMaterial = SpriteRenderer.material;

        collapseSFX = this is BossMonster
            ? $"event:/SFX/Monster/Boss_{ID}_Collapse"
            : $"event:/SFX/Monster/{ID}_Collapse";
        hurtSFX = this is BossMonster ? $"event:/SFX/Monster/Boss_{ID}_Hurt" : $"event:/SFX/Monster/{ID}_Hurt";
    }

    protected virtual void OnEnable()
    {
        isCollapsed = false;
        hp = MaxHp;
        collider2D.enabled = true;
        Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        Rigidbody2D.velocity = Vector3.zero;
        GameManager.Instance.enemyRunTimeSet.Add(gameObject);
        SpriteRenderer.sprite = defaultSprite;
        Animator.SetTrigger("AWAKE");
    }

    protected virtual void OnDisable()
    {
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);
        StopAllCoroutines();
    }

    public virtual void ReceiveHit(int damage, HitType hitType = HitType.Normal)
    {
        if (isCollapsed)
        {
            return;
        }

        hp -= damage;
        Rigidbody2D.velocity = Vector3.zero;

        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>()
            .SetText(damage.ToString(), hitType);
        ObjectManager.Instance.PopObject("Effect_Hit",
            transform.position + (Vector3.Normalize(Wakgood.Instance.transform.position - transform.position) * .5f));

        if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[36]))
        {
            if ((float)hp / MaxHp <= 0.1f * DataManager.Instance.wgItemInven.itemCountDic[36])
            {
                ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up)
                    .GetComponent<AnimatedText>().SetText("처형", Color.red);
                RuntimeManager.PlayOneShot(hurtSFX, transform.position);
                StopAllCoroutines();
                Collapse();
                return;
            }
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());

        _ReceiveHit();

        RuntimeManager.PlayOneShot("event:/SFX/Monster/Hit", transform.position);

        switch (hp)
        {
            case > 0:
                // Animator.SetTrigger("AHYA");
                break;
            case <= 0:
                RuntimeManager.PlayOneShot(hurtSFX, transform.position);
                StopAllCoroutines();
                Collapse();
                break;
        }
    }

    protected virtual void _ReceiveHit() { }

    protected virtual void Collapse()
    {
        SpriteRenderer.material = originalMaterial;

        if (DataManager.Instance.CurGameData.killedOnceMonster[ID] == false)
        {
            if (Collection.Instance != null)
            {
                Collection.Instance.Collect(this);
            }

            DataManager.Instance.CurGameData.killedOnceMonster[ID] = true;
            DataManager.Instance.SaveGameData();
        }

        isCollapsed = true;
        RuntimeManager.PlayOneShot(collapseSFX, transform.position);

        collider2D.enabled = false;
        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.bodyType = RigidbodyType2D.Static;
        // Animator.SetTrigger("COLLAPSE");
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);

        if (StageManager.Instance.CurrentRoom is NormalRoom)
        {
            onMonsterCollapse.Raise(transform);
            int randCount = Random.Range(0, 5 + 1);
            for (int i = 0; i < randCount; i++)
            {
                ObjectManager.Instance.PopObject("ExpOrb", transform);
            }

            randCount = Random.Range(0, 5 + 1);
            for (int i = 0; i < randCount; i++)
            {
                ObjectManager.Instance.PopObject("Goldu", transform);
            }
        }

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);
        gameObject.SetActive(false);
    }

    private IEnumerator FlashRoutine()
    {
        SpriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(.05f);
        SpriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }

    protected Vector3 GetRot()
    {
        return new Vector3(0, 0, (Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 0.8f),
            Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg) - 90);
    }

    protected Vector3 GetDirection()
    {
        return (Wakgood.Instance.transform.position - transform.position).normalized;
    }

    protected bool IsWakgoodRight()
    {
        return Wakgood.Instance.transform.position.x > transform.position.x;
    }
}