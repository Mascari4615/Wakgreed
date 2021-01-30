using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class Traveller : MonoBehaviour
{
    // SingleTon
    private static Traveller instance;
    [HideInInspector] public static Traveller Instance { get { return instance; } }

    // Stat
    [HideInInspector] public int ad = 0;
    [HideInInspector] public int criticalChance = 0;
    protected int hp = 0;
    protected int hpMax = 0;
    protected int exp = 0;
    protected int necessaryExp = 0;
    protected int level = 0;
    protected float moveSpeed = 0;
    protected float attackCoolTime = 0;

    // Stuff
    protected float h = 0;
    protected float v = 0;
    protected bool isInputAttack;
    protected float t = 0;
    protected float t1 = 0;
    protected bool isReadyToAttack = false;
    protected bool isBleeding = false;
    protected int targetIndex = 4444;
    protected float attackPosGap = 1.5f;

    // GameObject, Component
    [SerializeField] protected GameObject legacyOfTheHero;

    [HideInInspector] public Rigidbody2D playerRB;
    protected Animator animator;
    protected AudioSource audioSource;
    protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Transform attackPositionParent;
    [SerializeField] public Transform attackPosition;
    [SerializeField] protected Transform weaponPosition;
    [SerializeField] protected AudioClip[] attackAudioClips;
    [SerializeField] protected JoyStick joyStick;
    [SerializeField] protected CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] protected Image hpBar;
    [SerializeField] protected Text hpText;
    [SerializeField] protected Image expBar;
    [SerializeField] protected Text expText;
    [SerializeField] protected Text levelText ;  
    [SerializeField] protected GameObject bloodingPanel;

    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    protected virtual void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void OnEnable()
    {
        print("Enable");
        instance = this;
        cinemachineTargetGroup.m_Targets[0].target = transform;
        Initialize();
    }

    public void Initialize()
    {
        print("Initialize");

        hp = 100;
        hpMax = hp;
        ad = 30;
        attackCoolTime = 0.3f;
        moveSpeed = 8;
        necessaryExp = 100;
        exp = 0;
        level = 0;
        criticalChance = 5;

        transform.position = Vector3.zero;
        
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        InteractionManager.Instance.nearInteractionObject = InteractiveObjectType.None;

        inventory.Clear();

        animator.SetBool("Run", false);

        hpBar.fillAmount = (float)hp / hpMax;
        hpText.text = hp + " / " + hpMax;

        expBar.fillAmount = (float)exp / necessaryExp;
        levelText.text = "Lv. " + level;
        expText.text = Mathf.Floor((float)exp / necessaryExp * 100) + "%";
    }

    protected virtual void Update()
    { 
        Move();

        if (isReadyToAttack == false)
        {
            t += Time.deltaTime;
            if (t >= attackCoolTime)
            {
                t = 0;
                isReadyToAttack = true;
            }
        }

        if ((isInputAttack || Input.GetKey(KeyCode.Space)) && isReadyToAttack)
        {
            Attack();
            isReadyToAttack = false;
        }

        Targeting();

        if (isBleeding)
        {
            if (t1 < 1f)
            {
                spriteRenderer.color = new Color(1, 1, 1, (float)100 / 255);
                t1 += Time.deltaTime;
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, 1);
                isBleeding = false;
                t1 = 0;
            }
        }
    }

    protected virtual void Move()
    {
        h = joyStick.inputValue.x;
        v = joyStick.inputValue.y;

        if (joyStick.isDraging)
        {
            playerRB.velocity = new Vector2(h, v).normalized * moveSpeed;      
            animator.SetBool("Run", true);       
        }
        else
        {
            playerRB.velocity = Vector2.zero;
            animator.SetBool("Run", false);
        }
    }

    public void AttackButtonDown()
    {
        isInputAttack = true;
    }
    public void AttackButtonUp()
    {
        isInputAttack = false;
    }
    protected virtual void Attack()
    {
        audioSource.clip = attackAudioClips[Random.Range(0, attackAudioClips.Length)];
        audioSource.Play();
    }

    protected void Targeting()
    {
        if (GameManager.Instance.monsters.Count > 0)
        {
            targetIndex = 4444;
            float currentDist = 0;
            float targetDist = 15;
            
            for (int i = 0; i < GameManager.Instance.monsters.Count; i++)
            {
                currentDist = Vector2.Distance(transform.position, GameManager.Instance.monsters[i].transform.position);
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, GameManager.Instance.monsters[i].transform.position - transform.position, currentDist, LayerMask.NameToLayer("Everything"));
                for (int j = 0; j < hit.Length; j++)
                {
                    if (hit[j].transform.CompareTag("Wall"))
                    {
                        break;
                    }
                    else if (hit[j].transform.CompareTag("Monster") && (targetDist >= currentDist))
                    {                     
                        targetIndex = i;
                        targetDist = currentDist; 
                    }
                }
            }
        }
        else
        {
            targetIndex = 4444; // 타겟팅 안됨
        }

        if (targetIndex == 4444)
        {
            cinemachineTargetGroup.m_Targets[1].target = null;
            attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(joyStick.inputValue.y, joyStick.inputValue.x) * Mathf.Rad2Deg - 90);
            attackPosition.transform.localPosition = new Vector3(0, attackPosGap, 0);

            if (h > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);       
            }
            else if (h < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, GameManager.Instance.monsters[targetIndex].transform.position - transform.position, Color.red);

            cinemachineTargetGroup.m_Targets[1].target = GameManager.Instance.monsters[targetIndex].transform;
            attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - GameManager.Instance.monsters[targetIndex].transform.position.y, transform.position.x - GameManager.Instance.monsters[targetIndex].transform.position.x) * Mathf.Rad2Deg + 90);

            if (GameManager.Instance.monsters[targetIndex].transform.position.x >= transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (isBleeding)
        {
            return;
        }
        
        bloodingPanel.SetActive(false);
        bloodingPanel.SetActive(true);
        isBleeding = true;
        // ObjectManager.Instance.GetQueue(PoolType.animatedText, transform.position, damage + "", Color.red);
        hp -= damage;
        hpBar.fillAmount = (float)hp / hpMax;
        hpText.text = hp + " / " + hpMax;

        if (hp <= 0)
        {
            Debug.Log("* Don't lose hope.");
            Instantiate(legacyOfTheHero, transform);

            hpBar.fillAmount = 0;
            hpText.text = 0 + " / " + hpMax;

            animator.SetTrigger("Died");
            
            playerRB.bodyType = RigidbodyType2D.Static;

            StartCoroutine(GameManager.Instance.GameOver());
            this.enabled = false;
        }
    }

    public void AcquireExp(int expValue)
    {
        exp += expValue;
        
        if (exp >= necessaryExp)
        {
            LevelUp();
            level += 1;
            exp = 0;
            necessaryExp += 150;  
        }

        expBar.fillAmount = (float)exp / necessaryExp;
        levelText.text = "Lv. " + level;
        expText.text = Mathf.Floor((float)exp / necessaryExp * 100) + "%";
    }

    protected void LevelUp()
    {
        ObjectManager.Instance.GetQueue(PoolType.Smoke, transform);
        AbilityManager.Instance.LevelUp();
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.tag;
        switch (tag)
        {
            case "UpperDoor":
                StartCoroutine(GameManager.Instance.MigrateRoom("Up"));    
                break;
            case "LowerDoor":
                StartCoroutine(GameManager.Instance.MigrateRoom("Down"));   
                break;
            case "LeftDoor":
                StartCoroutine(GameManager.Instance.MigrateRoom("Left"));
                break;
            case "RightDoor":
                StartCoroutine(GameManager.Instance.MigrateRoom("Right"));
                break;
        }
    }
}
