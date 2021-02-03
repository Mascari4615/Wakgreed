using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;

public class Traveller : MonoBehaviour
{
    private static Traveller instance;
    [HideInInspector] public static Traveller Instance { get { return instance; } }

    private const int DEFAULT_HP_MAX = 30;
    private int hpMax;
    private int hp;

    private const int DEFAULT_AD = 3;
    [HideInInspector] public int ad;

    private const int DEFAULT_CRITICAL_CHANCE = 0;
    [HideInInspector] public int criticalChance;

    private const int DEFAULT_MOVE_SPEED = 8;
    [HideInInspector] public float moveSpeed;

    private int exp;
    private int requiredExp;
    private int level;
    
    private const float COOL_DOWN = 1f;
    private float currentCoolDown;
    private bool isHealthy = true;

    private const float DEFAULT_ATTACK_COOL_DOWN = 0.3f;
    private const float DEFAULT_SKILL_0_COOL_DOWN = 3f;
    private const float DEFAULT_SKILL_1_COOL_DOWN = 3f;
    private const float DEFAULT_SKILL_2_COOL_DOWN = 3f;

    [HideInInspector] public float attackCoolDown;
    private float skill0CoolDown;
    private float skill1CoolDown;
    private float skill2CoolDown;

    private float currentAttackCoolDown;
    private float currentSkill0CoolDown;
    private float currentSkill1CoolDown;
    private float currentSkill2CoolDown;

    private bool canAttack = true;
    private bool canUseSkill0 = true;
    private bool canUseSkill1 = true;
    private bool canUseSkill2 = true;

    private bool isInputtingAttack = false;
    private bool isInputtingSkill0 = false;
    private bool isInputtingSkill1 = false;
    private bool isInputtingSkill2 = false;

    protected float h = 0;
    private float v = 0;

    private float attackPosGap = 1.5f;

    // GameObject, Component
    private Rigidbody2D playerRB;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    [SerializeField] protected Transform attackPositionParent;
    [SerializeField] public Transform attackPosition;
    [SerializeField] protected Transform weaponPosition;
    [SerializeField] private AudioClip[] attackAudioClips;
    [SerializeField] private AudioClip[] skill0AudioClips;
    [SerializeField] private AudioClip[] skill1AudioClips;
    [SerializeField] private AudioClip[] skill2AudioClips;
    [SerializeField] protected JoyStick joyStick;
    [SerializeField] private CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] private Image hpBar;
    [SerializeField] private Text hpText;
    [SerializeField] private Image expBar;
    [SerializeField] private Text expText;
    [SerializeField] private Text levelText ;  
    [SerializeField] private GameObject bloodingPanel;

    [SerializeField] private EventTrigger attackButtonEventTrigger;
    [SerializeField] private EventTrigger skill0ButtonEventTrigger;
    [SerializeField] private EventTrigger skill1ButtonEventTrigger;
    [SerializeField] private EventTrigger skill2ButtonEventTrigger;
    EventTrigger.Entry attackPointerDown = new EventTrigger.Entry();
    EventTrigger.Entry attackPointerEnter = new EventTrigger.Entry();
    EventTrigger.Entry attackPointerUp = new EventTrigger.Entry();
    EventTrigger.Entry attackPointerExit = new EventTrigger.Entry();
    EventTrigger.Entry skill0PointerDown = new EventTrigger.Entry();
    EventTrigger.Entry skill1PointerDown = new EventTrigger.Entry();
    EventTrigger.Entry skill2PointerDown = new EventTrigger.Entry();

    private GameObject target;
    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    protected virtual void Awake()
    {
        Debug.Log(name + " : Awake");

        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        attackPointerDown.eventID = EventTriggerType.PointerDown;
        // attackPointerEnter.eventID = EventTriggerType.PointerEnter;
        attackPointerUp.eventID = EventTriggerType.PointerUp;
        // attackPointerExit.eventID = EventTriggerType.PointerExit;
        attackPointerDown.callback.AddListener((PointerEventData) => {isInputtingAttack = true;});
        // attackPointerEnter.callback.AddListener((PointerEventData) => {isInputtingattack = true;});
        attackPointerUp.callback.AddListener((PointerEventData) => {isInputtingAttack = false;});
        // attackPointerExit.callback.AddListener((PointerEventData) => {isInputtingattack = false;});

        skill0PointerDown.eventID = EventTriggerType.PointerDown;
        skill1PointerDown.eventID = EventTriggerType.PointerDown;
        skill2PointerDown.eventID = EventTriggerType.PointerDown;
        skill0PointerDown.callback.AddListener((PointerEventData) => {isInputtingSkill0 = true;});
        skill1PointerDown.callback.AddListener((PointerEventData) => {isInputtingSkill1 = true;});
        skill2PointerDown.callback.AddListener((PointerEventData) => {isInputtingSkill2 = true;});
    }

    private void OnEnable()
    {
        Debug.Log(name + " : OnEnable");
        Initialize();
    }

    public void Initialize()
    {
        instance = this;

        transform.position = Vector3.zero;

        hpMax = DEFAULT_HP_MAX;
        hp = hpMax;;
        ad = DEFAULT_AD;
        criticalChance = DEFAULT_CRITICAL_CHANCE;
        moveSpeed = DEFAULT_MOVE_SPEED;   
        exp = 0;
        requiredExp = 100;
        level = 0;
        currentCoolDown = 0;
        isHealthy = true;
        currentAttackCoolDown = 0;
        currentSkill0CoolDown = 0;
        currentSkill1CoolDown = 0;
        currentSkill2CoolDown = 0;
        attackCoolDown = DEFAULT_ATTACK_COOL_DOWN;
        skill0CoolDown = DEFAULT_SKILL_0_COOL_DOWN;
        skill1CoolDown = DEFAULT_SKILL_1_COOL_DOWN;
        skill2CoolDown = DEFAULT_SKILL_2_COOL_DOWN;
        canAttack = true;
        canUseSkill0 = true;
        canUseSkill1 = true;
        canUseSkill2 = true;
        isInputtingAttack = false;
        isInputtingSkill0 = false;
        isInputtingSkill1 = false;
        isInputtingSkill2 = false;
        h = 0;
        v = 0;
       
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = transform;
        animator.SetTrigger("WakeUp");
        animator.SetBool("Run", false);

        InteractionManager.Instance.nearInteractionObject = InteractiveObjectType.None;
        inventory.Clear();

        hpBar.fillAmount = (float)hp / hpMax;
        hpText.text = hp + " / " + hpMax;

        expBar.fillAmount = (float)exp / requiredExp;
        levelText.text = "Lv. " + level;
        expText.text = Mathf.Floor((float)exp / requiredExp * 100) + "%";

        attackButtonEventTrigger.triggers.Clear();
        attackButtonEventTrigger.triggers.Add(attackPointerDown);
        // attackButtonEventTrigger.triggers.Add(attackPointerEnter);
        attackButtonEventTrigger.triggers.Add(attackPointerUp);
        // attackButtonEventTrigger.triggers.Add(attackPointerExit);
        
        skill0ButtonEventTrigger.triggers.Clear();
        skill1ButtonEventTrigger.triggers.Clear();
        skill2ButtonEventTrigger.triggers.Clear();
        skill0ButtonEventTrigger.triggers.Add(skill0PointerDown);
        skill1ButtonEventTrigger.triggers.Add(skill1PointerDown);
        skill2ButtonEventTrigger.triggers.Add(skill2PointerDown);
    }

    protected virtual void Update()
    {
        Move();
        Targeting();

        CheckCoolDown(ref isHealthy, ref currentCoolDown, COOL_DOWN);
        CheckCoolDown(ref canAttack, ref currentAttackCoolDown, attackCoolDown);
        CheckCoolDown(ref canUseSkill0, ref currentSkill0CoolDown, skill0CoolDown);
        CheckCoolDown(ref canUseSkill1, ref currentSkill1CoolDown, skill1CoolDown);
        CheckCoolDown(ref canUseSkill2, ref currentSkill2CoolDown, skill2CoolDown);

        if (isHealthy == true) spriteRenderer.color = Color.white;
        else if (isHealthy == false) spriteRenderer.color = new Color(1, 1, 1, (float)100 / 255);

        if ((isInputtingAttack || Input.GetKey(KeyCode.Space)) && canAttack) Attack();
        isInputtingAttack = false;

        if (isInputtingSkill0 && canUseSkill0) Skill0();
        isInputtingSkill0 = false;

        if (isInputtingSkill1 && canUseSkill1) Skill1();
        isInputtingSkill1 = false;

        if (isInputtingSkill2 && canUseSkill2) Skill2();
        isInputtingSkill2 = false;
    }

    private void CheckCoolDown(ref bool coolDownTarget, ref float currentCoolDown, float coolDown)
    {
        if (coolDownTarget == false)
        {
            currentCoolDown += Time.deltaTime;
            if (currentCoolDown >= coolDown)
            {
                coolDownTarget = true;
                currentCoolDown = 0;
            }
        }
    }

    private void Move()
    {
        h = joyStick.inputValue.x;
        v = joyStick.inputValue.y;
        Vector3 moveDirection = new Vector2(h, v).normalized;

        if (joyStick.isDraging == true)
        {
            playerRB.velocity = moveDirection * moveSpeed;      
            animator.SetBool("Run", true);       
        }
        else if (joyStick.isDraging == false)
        {
            playerRB.velocity = Vector2.zero;
            animator.SetBool("Run", false);
        }

        if (target != null)
        {
            if (target.transform.position.x >= transform.position.x) transform.localScale = new Vector3(1, 1, 1); //오른쪽
            else if (target.transform.position.x < transform.position.x) transform.localScale = new Vector3(-1, 1, 1); // 왼쪽
        }
        else if (target == null)
        {
            if (h > 0) transform.localScale = new Vector3(1, 1, 1);       
            else if (h < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    protected virtual void Attack()
    {
        Debug.Log(name + " : Attack");

        audioSource.clip = attackAudioClips[Random.Range(0, attackAudioClips.Length)];
        audioSource.Play();
        canAttack = false;
    }

    protected virtual void Skill0()
    {
        Debug.Log(name + " : Skill0");

        audioSource.clip = skill0AudioClips[Random.Range(0, skill0AudioClips.Length)];
        audioSource.Play();
        canUseSkill0 = false;
    }

    protected virtual void Skill1()
    {
        Debug.Log(name + " : Skill1");

        audioSource.clip = skill1AudioClips[Random.Range(0, skill1AudioClips.Length)];
        audioSource.Play();
        canUseSkill1 = false;
    }

    protected virtual void Skill2()
    {
        Debug.Log(name + " : Skill2");

        audioSource.clip = skill2AudioClips[Random.Range(0, skill2AudioClips.Length)];
        audioSource.Play();
        canUseSkill2 = false;
    }

    private void Targeting()
    {
        target = null;
        float targetDist = 4444;
        float currentDist = 0;
        
        if (GameManager.Instance.monsters.Count > 0)
        {
            foreach (var monster in GameManager.Instance.monsters)
            {
                currentDist = Vector2.Distance(transform.position, monster.transform.position);
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, monster.transform.position - transform.position, currentDist, LayerMask.NameToLayer("Everything"));

                foreach (var hitObject in hit)
                {
                    if (hitObject.transform.CompareTag("Wall")) break;
                    else if (hitObject.transform.CompareTag("Monster") && (currentDist < targetDist))
                    {
                        target = monster;
                        targetDist = currentDist;
                    }
                }
            }           
        }

        if (target != null) 
        {
            cinemachineTargetGroup.m_Targets[1].target = target.transform;
            attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - target.transform.position.y, transform.position.x - target.transform.position.x) * Mathf.Rad2Deg + 90);
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
        }
        else
        {
            cinemachineTargetGroup.m_Targets[1].target = null;
            attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(joyStick.inputValue.y, joyStick.inputValue.x) * Mathf.Rad2Deg - 90);
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (isHealthy == false) return;

        hp -= damage;
        isHealthy = false;

        hpBar.fillAmount = (float)hp / hpMax;
        hpText.text = $"{hp} / {hpMax}";

        bloodingPanel.SetActive(false);
        bloodingPanel.SetActive(true);

        if (hp <= 0) Collapse();
    }

    private void Collapse()
    {
        Debug.Log($"{name} : Collapse");

        this.enabled = false;
        playerRB.bodyType = RigidbodyType2D.Static;
        StartCoroutine(GameManager.Instance.GameOver());

        hpBar.fillAmount = 0;
        hpText.text = $"0 / {hpMax}";

        animator.SetTrigger("Collapse");
    }

    public void AcquireExp(int value)
    {
        exp += value;
        if (exp >= requiredExp) LevelUp();

        expBar.fillAmount = (float)exp / requiredExp;
        expText.text = Mathf.Floor((float)exp / requiredExp * 100) + "%";
    }

    private void LevelUp()
    {
        level++;
        exp = exp - requiredExp;
        requiredExp += 100; 

        levelText.text = $"Lv. {level}";
        
        ObjectManager.Instance.GetQueue(PoolType.Smoke, transform);
        AbilityManager.Instance.LevelUp();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UpperDoor")) StartCoroutine(GameManager.Instance.MigrateRoom("Up"));
        else if (other.CompareTag("LowerDoor")) StartCoroutine(GameManager.Instance.MigrateRoom("Down")); 
        else if (other.CompareTag("LeftDoor")) StartCoroutine(GameManager.Instance.MigrateRoom("Left")); 
        else if (other.CompareTag("RightDoor")) StartCoroutine(GameManager.Instance.MigrateRoom("Right")); 
    }
}
