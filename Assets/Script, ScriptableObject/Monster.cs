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

    //private bool isHpBarSideEffectOn = false;
    
    [SerializeField] protected Animator monsterAnimator = null;
    public Rigidbody2D monsterRigidbody2D = null;
    [SerializeField] protected SpriteRenderer spriteRenderer = null;
    [SerializeField] protected CircleCollider2D circleCollider2D = null;
    [SerializeField] protected GameObject hpBarGameObject = null;
    [SerializeField] protected GameObject yellowParent = null;
    [SerializeField] protected SpriteRenderer yellow = null;
    [SerializeField] protected GameObject redParent = null;
    [SerializeField] protected AudioSource audioSource = null;
    [SerializeField] protected AudioClip[] audioClips = null;

    private void OnEnable()
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
        circleCollider2D.enabled = true;

        _OnEnable();
    }

    protected abstract void _OnEnable();

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

    protected void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<TravellerController>().ReceiveDamage(ad);
        }
    }

    public virtual void ReceiveDamage(int damage)
    {
        int rand = Random.Range(0, 100);
        string type = "";
        
        if (rand < TravellerCriticalChance.RuntimeValue)
        {
            damage *= 2;
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
            DropItem(transform.position);

            audioSource.clip = audioClips[1];
            audioSource.Play();
        }  
    }

    protected void DropItem(Vector3 diedPosition)
    {
        ObjectManager.Instance.GetQueue(PoolType.Smoke, diedPosition);

        for (int i = 0; i < 3; i++)
        {
            Vector3 randPos1 = new Vector3(Random.Range(-1f, 2f), Random.Range(-1f, 2f), 0);
            ObjectManager.Instance.GetQueue(PoolType.Exp, transform).GetComponent<Loot>().waitPosition = diedPosition + randPos1;
        }

        if (GameManager.Instance.currentRoom.isCleared)
        {
            Vector3 randPos1 = new Vector3(Random.Range(-1f, 2f), Random.Range(-1f, 2f), 0);
            GameObject g = ObjectManager.Instance.GetQueue(PoolType.Item, transform);
            g.GetComponent<ItemGameObject>().waitPosition = diedPosition + randPos1;
        }
    }
}
