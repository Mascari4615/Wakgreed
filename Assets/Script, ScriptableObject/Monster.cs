using UnityEngine;

public abstract class Monster : PoolingObject
{
    [SerializeField] private IntVariable TravellerCriticalChance;
    [SerializeField] protected EnemyRunTimeSet enemyRunTimeSet;
    [SerializeField] protected int _hp, _ad; // 초기 값, 인스펙터에서 _안보임
    [HideInInspector] public int hp, ad; // 실제 값

    protected bool isBoss = false;
    protected bool isAlive = true;
    private float lastTime;
    
    protected int hpMax;
    [SerializeField] protected float _moveSpeed;
    [HideInInspector] public float moveSpeed;

    [SerializeField] protected Animator monsterAnimator;
    public Rigidbody2D monsterRigidbody2D;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private GameObject hpBarGameObject;
    [SerializeField] private GameObject yellowParent;
    [SerializeField] private SpriteRenderer yellow;
    [SerializeField] private GameObject redParent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    protected virtual void OnEnable()
    {
        Debug.Log($"{name} : OnEnable");
        isAlive = true;
        lastTime = 0;
        hp = _hp;
        ad = _ad;
        hpMax = hp;
        hpBarGameObject.SetActive(true);
        redParent.transform.localScale = Vector3.one;
        yellowParent.transform.localScale = Vector3.one;
        moveSpeed = _moveSpeed;
        circleCollider2D.enabled = true;;
    }

    protected virtual void Update()
    {
        if (!isAlive)
        {
            lastTime += Time.deltaTime;
            if (lastTime >= 0.3f)
            {
                InsertQueue();
            }
            return;
        }

        redParent.transform.localScale = new Vector3(Mathf.Lerp(redParent.transform.localScale.x, (float)hp/hpMax, Time.deltaTime * 30f), 1, 1);

        yellow.color = new Color(1, 1, Mathf.Lerp(0, 1, Time.deltaTime * 30f));
        yellowParent.transform.localScale = new Vector3(Mathf.Lerp(yellowParent.transform.localScale.x, redParent.transform.localScale.x, Time.deltaTime * 15f), 1, 1);
            
        if (yellowParent.transform.localScale.x - 0.01f <= redParent.transform.localScale.x)
        {
            yellow.color = new Color(1, 1, 0);
            yellowParent.transform.localScale = new Vector3(redParent.transform.localScale.x, 1, 1);
        }
    }

    protected virtual void Attack()
    {

    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<TravellerController>().ReceiveDamage(ad);
        }
    }

    public virtual void ReceiveDamage(int damage)
    {
        string type = "";
        
        if (Random.Range(0, 100) < TravellerCriticalChance.RuntimeValue)
        {
            damage *= 4;
            type = "Critical";
        }

        ObjectManager.Instance.GetQueue(PoolType.AnimatedText, transform.position, damage + "", type);
        hp -= damage;

        if (hp > 0)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
        }
        else if (hp <= 0)
        {
            isAlive = false;

            hpBarGameObject.SetActive(false);
            circleCollider2D.enabled = false;
            monsterRigidbody2D.velocity = Vector2.zero;
            monsterAnimator.SetTrigger("Died");

            GameManager.Instance.AcquireKillCount();
            if (isBoss) GameManager.Instance.currentRoom.BossClear();
            else GameManager.Instance.currentRoom.CheckMonsterCount();
            enemyRunTimeSet.Remove(gameObject);
            DropItem();

            audioSource.clip = audioClips[1];
            audioSource.Play();
        }  
    }

    private void DropItem()
    {
        ObjectManager.Instance.GetQueue(PoolType.Smoke, transform.position);

        int randCount = Random.Range(0, 5);
        for (int i = 0; i < 3; i++) ObjectManager.Instance.GetQueue(PoolType.Exp, transform.position);

        if (GameManager.Instance.currentRoom.isCleared) ObjectManager.Instance.GetQueue(PoolType.Item, transform.position);
    }
}
