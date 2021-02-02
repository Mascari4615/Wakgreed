using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;

public class Traveller : MonoBehaviour
{
    // SingleTon
    private static Traveller instance;
    [HideInInspector] public static Traveller Instance { get { return instance; } }

    // Stat
    [HideInInspector] public int ad = 0;
    [HideInInspector] public int criticalChance = 0;
    private int hp = 0;
    private int hpMax = 0;
    private int exp = 0;
    private int necessaryExp = 0;
    private int level = 0;
    public float moveSpeed = 0;

    private const float coolDown = 1f;
    private float currentCoolDown = 0f;
    private bool isHealthy = true;
    
    public float basicAttackCoolDown = 0.3f;
    private float currentBasicAttackCoolDown = 0f;
    private bool isBasicAttackReady = true;
    private bool isInputtingBasicAttack = false;

    private float skill0CoolDown = 3f;
    private float currenSkill0CoolDown = 0f;
    private bool isSkill0Ready = true;
    private bool isInputtingSkill0 = false;

    private float skill1CoolDown = 3f;
    private float currenSkill1CoolDown = 0f;
    private bool isSkill1Ready = true;
    private bool isInputtingSkill1 = false;

    private float skill2CoolDown = 3f;
    private float currenSkill2CoolDown = 0f;
    private bool isSkill2Ready = true;
    private bool isInputtingSkill2 = false;

    // Stuff
    protected float h = 0;
    private float v = 0;
    
    private GameObject target;
    private float attackPosGap = 1.5f;

    // GameObject, Component
    [SerializeField] private GameObject legacyOfTheHero;

    [HideInInspector] public Rigidbody2D playerRB;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    [SerializeField] protected Transform attackPositionParent;
    [SerializeField] public Transform attackPosition;
    [SerializeField] protected Transform weaponPosition;
    [SerializeField] private AudioClip[] basicAttackAudioClips;
    [SerializeField] private AudioClip[] skill0AudioClips;
    [SerializeField] private AudioClip[] skill1AudioClips;
    [SerializeField] private AudioClip[] skill2AudioClips;
    [SerializeField] protected JoyStick joyStick;
    [SerializeField] private EventTrigger basicAttackButtonEventTrigger;
    [SerializeField] private EventTrigger skill0ButtonEventTrigger;
    [SerializeField] private EventTrigger skill1ButtonEventTrigger;
    [SerializeField] private EventTrigger skill2ButtonEventTrigger;
    [SerializeField] private CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] private Image hpBar;
    [SerializeField] private Text hpText;
    [SerializeField] private Image expBar;
    [SerializeField] private Text expText;
    [SerializeField] private Text levelText ;  
    [SerializeField] private GameObject bloodingPanel;

    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    EventTrigger.Entry basicAttackPointerDown = new EventTrigger.Entry();
    EventTrigger.Entry basicAttackPointerEnter = new EventTrigger.Entry();
    EventTrigger.Entry basicAttackPointerUp = new EventTrigger.Entry();
    EventTrigger.Entry basicAttackPointerExit = new EventTrigger.Entry();

    EventTrigger.Entry skill0PointerDown = new EventTrigger.Entry();
    EventTrigger.Entry skill1PointerDown = new EventTrigger.Entry();
    EventTrigger.Entry skill2PointerDown = new EventTrigger.Entry();

    protected virtual void Awake()
    {
        Debug.Log(name + " : Awake");

        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        basicAttackPointerDown.eventID = EventTriggerType.PointerDown;
        basicAttackPointerDown.callback.AddListener((PointerEventData) => {isInputtingBasicAttack = true;});
        basicAttackPointerEnter.eventID = EventTriggerType.PointerEnter;
        basicAttackPointerEnter.callback.AddListener((PointerEventData) => {isInputtingBasicAttack = true;});
        basicAttackPointerExit.eventID = EventTriggerType.PointerExit;
        basicAttackPointerExit.callback.AddListener((PointerEventData) => {isInputtingBasicAttack = false;});
        basicAttackPointerUp.eventID = EventTriggerType.PointerUp;
        basicAttackPointerUp.callback.AddListener((PointerEventData) => {isInputtingBasicAttack = false;});

        skill0PointerDown.eventID = EventTriggerType.PointerDown;
        skill0PointerDown.callback.AddListener((PointerEventData) => {isInputtingSkill0 = true;});
        skill1PointerDown.eventID = EventTriggerType.PointerDown;
        skill1PointerDown.callback.AddListener((PointerEventData) => {isInputtingSkill1 = true;});
        skill2PointerDown.eventID = EventTriggerType.PointerDown;
        skill2PointerDown.callback.AddListener((PointerEventData) => {isInputtingSkill2 = true;});

        attackPosition.transform.position = new Vector3(0, attackPosGap, 0);
    }

    private void OnEnable()
    {
        Debug.Log(name + " : OnEnable");

        instance = this;
        cinemachineTargetGroup.m_Targets[0].target = transform;

        basicAttackButtonEventTrigger.triggers.Clear();
        basicAttackButtonEventTrigger.triggers.Add(basicAttackPointerDown);
        //basicAttackButtonEventTrigger.triggers.Add(basicAttackPointerEnter);
        //basicAttackButtonEventTrigger.triggers.Add(basicAttackPointerExit);
        basicAttackButtonEventTrigger.triggers.Add(basicAttackPointerUp);

        skill0ButtonEventTrigger.triggers.Clear();
        skill0ButtonEventTrigger.triggers.Add(skill0PointerDown);

        skill1ButtonEventTrigger.triggers.Clear();
        skill1ButtonEventTrigger.triggers.Add(skill1PointerDown);

        skill2ButtonEventTrigger.triggers.Clear();
        skill2ButtonEventTrigger.triggers.Add(skill2PointerDown);

        Initialize();
    }

    public void Initialize()
    {
        Debug.Log(name + " : Initialize");

        hp = 30;
        hpMax = hp;
        ad = 3;
        currentCoolDown = coolDown;
        currentBasicAttackCoolDown = basicAttackCoolDown;
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
        Targeting();

        CheckCoolDown(ref isBasicAttackReady, ref currentBasicAttackCoolDown, basicAttackCoolDown);
        if ((isInputtingBasicAttack || Input.GetKeyDown(KeyCode.Space)) && isBasicAttackReady) BasicAttack();

        CheckCoolDown(ref isHealthy, ref currentCoolDown, coolDown);
        if (isHealthy == true) spriteRenderer.color = new Color(1, 1, 1, 1);
        else if (isHealthy == false) spriteRenderer.color = new Color(1, 1, 1, (float)100 / 255);

        CheckCoolDown(ref isSkill0Ready, ref currenSkill0CoolDown, skill0CoolDown);
        if (isInputtingSkill0 && isSkill0Ready) Skill0();
        else isInputtingSkill0 = false;
        CheckCoolDown(ref isSkill1Ready, ref currenSkill1CoolDown, skill1CoolDown);
        if (isInputtingSkill1 && isSkill1Ready) Skill1();
        else isInputtingSkill1 = false;
        CheckCoolDown(ref isSkill2Ready, ref currenSkill2CoolDown, skill2CoolDown);
        if (isInputtingSkill2 && isSkill2Ready) Skill2();
        else isInputtingSkill2 = false;
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

    protected virtual void Move()
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

    protected virtual void BasicAttack()
    {
        Debug.Log(name + " : Attack");
        audioSource.clip = basicAttackAudioClips[Random.Range(0, basicAttackAudioClips.Length)];
        audioSource.Play();

        isBasicAttackReady = false;
    }

    protected virtual void Skill0()
    {
        Debug.Log(name + " : Skill0");
        audioSource.clip = skill0AudioClips[Random.Range(0, skill0AudioClips.Length)];
        audioSource.Play();

        isSkill0Ready = false;
    }

    protected virtual void Skill1()
    {
        Debug.Log(name + " : Skill1");
        audioSource.clip = skill1AudioClips[Random.Range(0, skill1AudioClips.Length)];
        audioSource.Play();

        isSkill1Ready = false;
    }

    protected virtual void Skill2()
    {
        Debug.Log(name + " : Skill2");
        audioSource.clip = skill2AudioClips[Random.Range(0, skill2AudioClips.Length)];
        audioSource.Play();

        isSkill2Ready = false;
    }

    private void Targeting()
    {
        if (GameManager.Instance.monsters.Count > 0)
        {
            target = null;
            float targetDist = 4444;
            float currentDist = 0;

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
        else
        {
            target = null;
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
        
        bloodingPanel.SetActive(false);
        bloodingPanel.SetActive(true);
        isHealthy = false;

        // ObjectManager.Instance.GetQueue(PoolType.animatedText, transform.position, damage + "", Color.red);

        hp -= damage;
        if (hp <= 0) Collapse();

        hpBar.fillAmount = (float)hp / hpMax;
        hpText.text = hp + $" / " + hpMax;
    }

    private void Collapse()
    {
        Debug.Log(name + " : Collapse");
        Debug.Log("* Don't lose hope.");
        Instantiate(legacyOfTheHero, transform);

        hpBar.fillAmount = 0;
        hpText.text = 0 + " / " + hpMax;

        animator.SetTrigger("Collapse");
        
        playerRB.bodyType = RigidbodyType2D.Static;

        StartCoroutine(GameManager.Instance.GameOver());
        this.enabled = false;
    }

    public void AcquireExp(int expValue)
    {
        exp += expValue;
        if (exp >= necessaryExp) LevelUp();

        expBar.fillAmount = (float)exp / necessaryExp;
        levelText.text = "Lv. " + level;
        expText.text = Mathf.Floor((float)exp / necessaryExp * 100) + "%";
    }

    private void LevelUp()
    {
        level += 1;
        exp = 0;
        necessaryExp += 150; 
        
        ObjectManager.Instance.GetQueue(PoolType.Smoke, transform);
        AbilityManager.Instance.LevelUp();
    }

    private void OnTriggerEnter2D(Collider2D other)
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
