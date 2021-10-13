using FMODUnity;
using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour, IHitable
{
    [SerializeField] private int baseHP, baseAD, baseMoveSpeed;
    protected int maxHP, HP, AD, moveSpeed;

    [SerializeField] private GameEvent OnMonsterCollapse;

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected new Rigidbody2D rigidbody2D;
    private CapsuleCollider2D capsuleCollider2D;

    private bool isCollapsed = false;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void OnEnable()
    {
        isCollapsed = false;

        maxHP = baseHP;
        HP = maxHP;
        AD = baseAD;
        moveSpeed = baseMoveSpeed;
        capsuleCollider2D.enabled = true;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        GameManager.Instance.EnemyRunTimeSet.Add(gameObject);
    }

    protected virtual void Update()
    {
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);
    }

    public virtual void ReceiveHit(int damage)
    {
        if (isCollapsed) return;

        HP -= damage;
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.AddForce((transform.position - Wakgood.Instance.transform.position).normalized * 3f, ForceMode2D.Impulse);

        ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText(damage.ToString(), TextType.Normal);

        if (HP > 0)
        {
            RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("("), 7) : name)}_Hurt", transform.position);
            animator.SetTrigger("AHYA");
        }
        else if (HP <= 0)
        {
            isCollapsed = true;

            RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("("), 7) : name)}_Hurt", transform.position);
            StopAllCoroutines();
            StartCoroutine(Collapse());
        }
    }

    protected virtual IEnumerator Collapse()
    {
        RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("("), 7) : name)}_Collapse", transform.position);

        capsuleCollider2D.enabled = false;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("COLLAPSE");
        GameManager.Instance.EnemyRunTimeSet.Remove(gameObject);

        if (StageManager.Instance.CurrentRoom.roomType == RoomType.Normal)
        {
            OnMonsterCollapse.Raise(transform);
            int randCount = Random.Range(0, 5);
            for (int i = 0; i < randCount; i++) ObjectManager.Instance.PopObject("ExpOrb", transform);
        }

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.Instance.EnemyRunTimeSet.Remove(gameObject);
        StopAllCoroutines();
    }
}
