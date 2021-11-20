using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class Monster : MonoBehaviour, IHitable
{
    [SerializeField] private int baseHp;
    [SerializeField] private int baseAD, baseMoveSpeed;
    public int MAXHp { get; protected set; }
    public int Hp { get; protected set; }
    public int ad { get; protected set; }
    protected int MoveSpeed;

    [SerializeField] private GameEvent onMonsterCollapse;

    protected SpriteRenderer SpriteRenderer;
    protected Animator Animator;
    protected new Rigidbody2D Rigidbody2D;
    private new Collider2D collider2D;

    private bool isCollapsed = false;
    
    private static readonly int ahya = Animator.StringToHash("AHYA");
    private static readonly int collapse = Animator.StringToHash("COLLAPSE");

    protected virtual void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
    }

    protected virtual void OnEnable()
    {
        isCollapsed = false;

        MAXHp = baseHp;
        Hp = MAXHp;
        ad = baseAD;
        MoveSpeed = baseMoveSpeed;
        collider2D.enabled = true;
        Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        GameManager.Instance.enemyRunTimeSet.Add(gameObject);
    }

    public void ReceiveHit(int damage)
    {
        if (isCollapsed) return;

        Hp -= damage;
        Rigidbody2D.velocity = Vector3.zero;
        Rigidbody2D.AddForce((transform.position - Wakgood.Instance.transform.position).normalized * 3f, ForceMode2D.Impulse);

        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText(damage.ToString(), TextType.Normal);
        ObjectManager.Instance.PopObject("HitEffect", transform.position + Vector3.Normalize(Wakgood.Instance.transform.position - transform.position) * .5f);

        _ReceiveHit();

        switch (Hp)
        {
            case > 0:
                RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", StringComparison.Ordinal), 7) : name)}_Hurt", transform.position);
                Animator.SetTrigger(ahya);
                break;
            case <= 0:
                isCollapsed = true;
                RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", StringComparison.Ordinal), 7) : name)}_Collapse", transform.position);
                StopAllCoroutines();
                StartCoroutine(Collapse());
                break;
        }
    }

    protected virtual void _ReceiveHit() { }

    protected virtual IEnumerator Collapse()
    {
        RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", StringComparison.Ordinal), 7) : name)}_Collapse", transform.position);

        collider2D.enabled = false;
        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.bodyType = RigidbodyType2D.Static;
        Animator.SetTrigger(collapse);
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);

        if (StageManager.Instance.CurrentRoom.roomType == RoomType.Normal)
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
}
