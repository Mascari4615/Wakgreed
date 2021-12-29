using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Monster : MonoBehaviour, IHitable
{
    [SerializeField] private Material flashMaterial;
    [SerializeField] protected GameEvent onMonsterCollapse;
    public string mobName;
    public int ID;
    [TextArea] public string description;
    public int MaxHp { get; protected set; }
    public int hp;
    [SerializeField] protected float MoveSpeed;
    protected bool isCollapsed;
    protected SpriteRenderer SpriteRenderer;
    public Sprite defaultSprite;
    protected Animator Animator;
    protected Rigidbody2D Rigidbody2D;
    protected new Collider2D collider2D;
    protected Material originalMaterial;
    private Coroutine flashRoutine;

    protected static WaitForSeconds ws01 = new WaitForSeconds(0.1f);

    protected virtual void Awake()
    {
        MaxHp = hp;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        originalMaterial = SpriteRenderer.material;
    }

    protected virtual void OnEnable()
    {
        isCollapsed = false;
        hp = MaxHp;
        collider2D.enabled = true;
        Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        GameManager.Instance.enemyRunTimeSet.Add(gameObject);
        SpriteRenderer.sprite = defaultSprite;
    }

    public virtual void ReceiveHit(int damage, HitType hitType = HitType.Normal)
    {
        if (isCollapsed) return;

        hp -= damage;
        Rigidbody2D.velocity = Vector3.zero;

        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText(damage.ToString(), hitType);
        ObjectManager.Instance.PopObject("Effect_Hit", transform.position + Vector3.Normalize(Wakgood.Instance.transform.position - transform.position) * .5f);

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashRoutine());

        _ReceiveHit();

        RuntimeManager.PlayOneShot($"event:/SFX/Monster/Hurt", transform.position);

        switch (hp)
        {
            case > 0:
                // Animator.SetTrigger("AHYA");
                break;
            case <= 0:
                RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", StringComparison.Ordinal), 7) : name)}_Collapse", transform.position);
                StopAllCoroutines();
                StartCoroutine(Collapse());
                break;
        }
    }

    protected virtual void _ReceiveHit() { }

    protected virtual IEnumerator Collapse()
    {
        SpriteRenderer.material = originalMaterial;

        if (DataManager.Instance.CurGameData.killedOnceMonster[ID] == false)
        {
            if (Collection.Instance != null)
                Collection.Instance.Collect(this);
            DataManager.Instance.CurGameData.killedOnceMonster[ID] = true;
            DataManager.Instance.SaveGameData();
        }

        isCollapsed = true;
        RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", StringComparison.Ordinal), 7) : name)}_Collapse", transform.position);

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
                ObjectManager.Instance.PopObject("ExpOrb", transform);
            randCount = Random.Range(0, 9 + 1);
            for (int i = 0; i < randCount; i++) 
                ObjectManager.Instance.PopObject("Goldu10", transform);
            randCount = Random.Range(0, 9 + 1);
            for (int i = 0; i < randCount; i++) 
                ObjectManager.Instance.PopObject("Goldu", transform);
            /*if (Random.Range(0, 100) > 90) 
                ObjectManager.Instance.PopObject("GreenHeart", transform);*/
        }

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        // yield return new WaitForSeconds(3f);
        yield return null;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);
        StopAllCoroutines();
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
        return new(0, 0, Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 0.8f),
            Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90);
    }

    protected Vector3 GetDirection()
    {
        return (Wakgood.Instance.transform.position - transform.position).normalized;  
    }
}
