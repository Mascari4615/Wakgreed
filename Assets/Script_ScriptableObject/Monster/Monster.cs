using FMODUnity;
using System.Collections;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    [SerializeField] private int baseHP, baseAD, baseMoveSpeed;
    protected int maxHP, HP, AD, moveSpeed;

    [SerializeField] private EnemyRunTimeSet EnemyRunTimeSet;
    [SerializeField] private GameEvent OnMonsterCollapse;

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected new Rigidbody2D rigidbody2D;
    private CapsuleCollider2D capsuleCollider2D;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void OnEnable()
    {
        maxHP = baseHP;
        HP = maxHP;
        AD = baseAD;
        moveSpeed = baseMoveSpeed;
        capsuleCollider2D.enabled = true;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        EnemyRunTimeSet.Add(gameObject);
    }

    protected virtual void Update()
    {
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);
    }

    public virtual void ReceiveDamage(int damage, TextType damageType = TextType.Normal)
    {
        //ObjectManager.Instance.GetQueue(PoolType.AnimatedText, transform.position).GetComponent<AnimatedText>().SetText(damage.ToString(), damageType);
        ObjectManager.Instance.PopObject("DamageText", transform).GetComponent<AnimatedText>().SetText(damage.ToString(), damageType);
        HP -= damage;
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.AddForce((transform.position - TravellerController.Instance.transform.position).normalized, ForceMode2D.Impulse);

        if (HP > 0)
        {
            RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("("), 7) : name)}_Hurt", transform.position);
            //animator.SetTrigger("Ahya");
        }
        else if (HP <= 0)
        {
            RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("("), 7) : name)}_Hurt", transform.position);

            StopAllCoroutines();
            StartCoroutine(Collapse());
        }
    }

    protected virtual IEnumerator Collapse()
    {
        capsuleCollider2D.enabled = false;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("Collapse");

        EnemyRunTimeSet.Remove(gameObject);
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
}
