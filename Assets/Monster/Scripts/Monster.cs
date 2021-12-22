using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Monster : MonoBehaviour, IHitable
{
    [SerializeField] private Material flashMaterial;
    [SerializeField] protected GameEvent onMonsterCollapse;
    public int MaxHp { get; protected set; }
    public int hp;
    [SerializeField] protected int MoveSpeed;
    protected bool isCollapsed;
    protected SpriteRenderer SpriteRenderer;
    protected Animator Animator;
    protected Rigidbody2D Rigidbody2D;
    protected new Collider2D collider2D;
    protected Material originalMaterial;
    private Coroutine flashRoutine;

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
    }

    public void ReceiveHit(int damage)
    {
        if (isCollapsed) return;

        hp -= damage;
        Rigidbody2D.velocity = Vector3.zero;
        Rigidbody2D.AddForce((transform.position - Wakgood.Instance.transform.position).normalized * 3f, ForceMode2D.Impulse);

        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText(damage.ToString(), TextType.Normal);
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
                Animator.SetTrigger("AHYA");
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

        isCollapsed = true;
        RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", StringComparison.Ordinal), 7) : name)}_Collapse", transform.position);

        collider2D.enabled = false;
        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.bodyType = RigidbodyType2D.Static;
        Animator.SetTrigger("COLLAPSE");
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);

        if (StageManager.Instance.CurrentRoom is NormalRoom)
        {
            onMonsterCollapse.Raise(transform);
            int randCount = Random.Range(0, 5);
            for (int i = 0; i < randCount; i++) ObjectManager.Instance.PopObject("ExpOrb", transform);
        }

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        yield return new WaitForSeconds(3f);
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

}
