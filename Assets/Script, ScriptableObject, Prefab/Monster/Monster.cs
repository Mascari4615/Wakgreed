using System.Collections;
using UnityEngine;
public abstract class Monster : MonoBehaviour
{
    private enum MonsterType
    {
        Normal, Boss
    }
    [SerializeField] private MonsterType monsterType;
    [SerializeField] protected int baseHP, baseAD, baseMoveSpeed;
    protected int maxHP, HP, AD, moveSpeed;

    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] protected EnemyRunTimeSet EnemyRunTimeSet;
    [SerializeField] private GameEvent OnMonsterCollapse;

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected new Rigidbody2D rigidbody2D;
    private CapsuleCollider2D capsuleCollider2D;
    private AudioSource audioSource;
    private GameObject hpBarGameObject;
    private GameObject yellowParent;
    private SpriteRenderer yellow;
    private GameObject redParent;

    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        hpBarGameObject = transform.GetChild(1).gameObject;
        yellowParent = hpBarGameObject.transform.GetChild(2).gameObject;
        yellow = yellowParent.transform.GetChild(0).GetComponent<SpriteRenderer>();
        redParent = hpBarGameObject.transform.GetChild(3).gameObject;
    }

    protected virtual void OnEnable()
    {
        Debug.Log($"{name} : OnEnable");

        maxHP = baseHP;
        HP = maxHP;
        AD = baseAD;
        moveSpeed = baseMoveSpeed;
        hpBarGameObject.SetActive(true);
        redParent.transform.localScale = Vector3.one;
        yellowParent.transform.localScale = Vector3.one;
        capsuleCollider2D.enabled = true;;
    }

    protected virtual void Update()
    {
        redParent.transform.localScale = new Vector3(Mathf.Lerp(redParent.transform.localScale.x, (float)HP/maxHP, Time.deltaTime * 30f), 1, 1);

        yellow.color = new Color(1, 1, Mathf.Lerp(0, 1, Time.deltaTime * 30f));
        yellowParent.transform.localScale = new Vector3(Mathf.Lerp(yellowParent.transform.localScale.x, redParent.transform.localScale.x, Time.deltaTime * 15f), 1, 1);
            
        if (yellowParent.transform.localScale.x - 0.01f <= redParent.transform.localScale.x)
        {
            yellow.color = new Color(1, 1, 0);
            yellowParent.transform.localScale = new Vector3(redParent.transform.localScale.x, 1, 1);
        }
    }

    public virtual void ReceiveDamage(int damage, DamageType damageType = DamageType.Normal)
    {
        ObjectManager.Instance.GetQueue(PoolType.AnimatedText, transform.position, damage.ToString(), damageType);
        HP -= damage;

        if (HP > 0)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
        }
        else if (HP <= 0)
        {
            audioSource.clip = audioClips[1];
            audioSource.Play();

            StartCoroutine(Collapse());
        }  
    }

    private IEnumerator Collapse()
    {
        hpBarGameObject.SetActive(false);
        capsuleCollider2D.enabled = false;
        rigidbody2D.velocity = Vector2.zero;
        animator.SetTrigger("Collapse");

        EnemyRunTimeSet.Remove(gameObject);
        if (GameManager.Instance.currentRoom.roomType == RoomType.Normal || monsterType == MonsterType.Boss)
        {
            OnMonsterCollapse.Raise();
            int randCount = Random.Range(0, 5);
            for (int i = 0; i < randCount; i++)
                ObjectManager.Instance.GetQueue(PoolType.Exp, transform.position);
        }
        
        ObjectManager.Instance.GetQueue(PoolType.Smoke, transform.position);

        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }
}
