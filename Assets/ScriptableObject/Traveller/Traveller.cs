using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;

[CreateAssetMenu]
public class Traveller : ScriptableObject, ISerializationCallbackReceiver
{
    public void OnBeforeSerialize() {}
    public void OnAfterDeserialize() {}
    public string tName;
    public int maxHP;
    [System.NonSerialized] public int HP;
    public int baseAD;
    [System.NonSerialized] public int AD = 1;
    public float baseAS;
    [System.NonSerialized] public float AS;
    public int baseCriticalChance;
    [System.NonSerialized] public int criticalChance;
    public int baseMoveSpeed;
    [System.NonSerialized] public float moveSpeed;
    public float abillity0CoolDown;
    public float abillity1CoolDown;
    public float abillity2CoolDown;
    private AudioSource audioSource;
    public AudioClip[] basicAttackAudioClips;
    public AudioClip[] abillity0AudioClips;
    public AudioClip[] abillity1AudioClips;
    public AudioClip[] abillity2AudioClips;

    private int exp;
    private int requiredExp;
    private int level;

    private float coolDown;
    
    private float currentCoolDown;
    private bool isHealthy = true;

    private float currentAttackCoolDown;
    private float currentSkill0CoolDown;
    private float currentSkill1CoolDown;
    private float currentSkill2CoolDown;

    private bool canAttack = true;
    private bool canUseAbility0 = true;
    private bool canUseAbility1 = true;
    private bool canUseAbility2 = true;
    private bool canInteraction = false;

    private bool isInputtingAttack = false;
    private bool isInputtingInteraction = false;

    [HideInInspector] public float h = 0;
    private float v = 0;

    private Rigidbody2D playerRB;
    private Animator animator;
    
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public Transform attackPositionParent;
    [HideInInspector] public Transform attackPosition;
    [HideInInspector] public Transform weaponPosition;
    [HideInInspector] public JoyStick joyStick;
    private CinemachineTargetGroup cinemachineTargetGroup;
    private Image hpBar;
    private Text hpText;
    private Image expBar;
    private Text expText;
    private Text levelText ;  
    private GameObject bloodingPanel;
    private GameObject interactionIcon = null;
    private GameObject attackIcon = null;

    private EventTrigger attackButtonEventTrigger;
    private EventTrigger skill0ButtonEventTrigger;
    private EventTrigger skill1ButtonEventTrigger;
    private EventTrigger skill2ButtonEventTrigger;
    private EventTrigger.Entry attackPointerDown = new EventTrigger.Entry();
    private EventTrigger.Entry attackPointerEnter = new EventTrigger.Entry();
    private EventTrigger.Entry attackPointerUp = new EventTrigger.Entry();
    private EventTrigger.Entry attackPointerExit = new EventTrigger.Entry();
    private EventTrigger.Entry skill0PointerDown = new EventTrigger.Entry();
    private EventTrigger.Entry skill1PointerDown = new EventTrigger.Entry();
    private EventTrigger.Entry skill2PointerDown = new EventTrigger.Entry();

    private GameObject target;
    public Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    private GameObject nearInteractiveObject;

    [HideInInspector] public TravellerController travellerController;
    private Transform transform;
    public TravellerFunctions travellerFunctions;

    public virtual void _Awake(TravellerController t)
    {
        travellerController = t;
        transform = t.transform;
        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        playerRB = t.GetComponent<Rigidbody2D>();
        animator = t.GetComponent<Animator>();
        audioSource = t.GetComponent<AudioSource>();
        spriteRenderer = t.GetComponent<SpriteRenderer>();

        attackPositionParent = t.attackPositionParent;
        attackPosition = t.attackPosition;
        weaponPosition = t.weaponPosition;
        joyStick = t.joyStick;
        cinemachineTargetGroup = t.cinemachineTargetGroup;
        hpBar = t.hpBar;
        hpText = t.hpText;
        expBar = t.expBar;
        expText = t.expText;
        levelText = t.levelText;
        bloodingPanel = t.bloodingPanel;
        interactionIcon = t.interactionIcon;
        attackIcon = t.attackIcon;
        attackButtonEventTrigger = t.attackButtonEventTrigger;
        skill0ButtonEventTrigger = t.skill0ButtonEventTrigger;
        skill1ButtonEventTrigger = t.skill1ButtonEventTrigger;
        skill2ButtonEventTrigger = t.skill2ButtonEventTrigger;

        attackPointerDown.eventID = EventTriggerType.PointerDown;
        // attackPointerEnter.eventID = EventTriggerType.PointerEnter;
        attackPointerUp.eventID = EventTriggerType.PointerUp;
        // attackPointerExit.eventID = EventTriggerType.PointerExit;
        attackPointerDown.callback.AddListener((PointerEventData) => {isInputtingAttack = true; isInputtingInteraction = true;});
        // attackPointerEnter.callback.AddListener((PointerEventData) => {isInputtingattack = true;});
        attackPointerUp.callback.AddListener((PointerEventData) => {isInputtingAttack = false;});
        // attackPointerExit.callback.AddListener((PointerEventData) => {isInputtingattack = false;});

        skill0PointerDown.eventID = EventTriggerType.PointerDown;
        skill1PointerDown.eventID = EventTriggerType.PointerDown;
        skill2PointerDown.eventID = EventTriggerType.PointerDown;
        skill0PointerDown.callback.AddListener((PointerEventData) => {if (canUseAbility0) Ability0();});
        skill1PointerDown.callback.AddListener((PointerEventData) => {if (canUseAbility1) Ability1();});
        skill2PointerDown.callback.AddListener((PointerEventData) => {if (canUseAbility2) Ability2();});
    }

    public virtual void Initialize(TravellerController t)
    {
        HP = maxHP;
        //AD = baseAD;
        AS = baseAS;
        criticalChance = baseCriticalChance;
        moveSpeed = baseMoveSpeed;   
        exp = 0;
        requiredExp = 100;
        level = 0;
        currentCoolDown = 0;
        isHealthy = true;
        currentAttackCoolDown = 0;
        currentSkill0CoolDown = 0;
        currentSkill1CoolDown = 0;
        currentSkill2CoolDown = 0;
        canAttack = true;
        canUseAbility0 = true;
        canUseAbility1 = true;
        canUseAbility2 = true;
        canInteraction = false;
        isInputtingAttack = false;
        isInputtingInteraction = false;
        h = 0;
        v = 0;
       
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = transform;
        animator.SetTrigger("WakeUp");
        animator.SetBool("Run", false);

        inventory.Clear();

        hpBar.fillAmount = (float)HP / maxHP;
        hpText.text = HP + " / " + maxHP;

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

        travellerFunctions.Initialize(this);
    }

    public virtual void _Update(TravellerController t)
    {
        if (Time.timeScale == 0) return;

        Move();
        Targeting();

        CheckCoolDown(ref isHealthy, ref currentCoolDown, coolDown);
        CheckCoolDown(ref canAttack, ref currentAttackCoolDown, AS);
        CheckCoolDown(ref canUseAbility0, ref currentSkill0CoolDown, abillity0CoolDown);
        CheckCoolDown(ref canUseAbility1, ref currentSkill1CoolDown, abillity1CoolDown);
        CheckCoolDown(ref canUseAbility2, ref currentSkill2CoolDown, abillity2CoolDown);

        if (isHealthy == true) spriteRenderer.color = Color.white;
        else if (isHealthy == false) spriteRenderer.color = new Color(1, 1, 1, (float)100 / 255);

        if (isInputtingInteraction && canInteraction) {Interaction(); isInputtingAttack = false;}
        else if ((isInputtingAttack || Input.GetKey(KeyCode.Space)) && canAttack) BasicAttack();
        isInputtingInteraction = false;

        travellerFunctions._Update(this);
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

    public void AddStat(TravellerStat stat, float value)
    {
        if (stat == TravellerStat.AD) AD =+ (int)value;
        else if (stat == TravellerStat.AS) AS =+ value;
    }

    public virtual void BasicAttack()
    {
        Debug.Log(name + " : Attack");

        audioSource.clip = basicAttackAudioClips[Random.Range(0, basicAttackAudioClips.Length)];
        audioSource.Play();
        canAttack = false;

        travellerFunctions.BasicAttack(this);
    }

    public virtual void Ability0()
    {
        Debug.Log(name + " : Ability0");
        HP += 300;

        audioSource.clip = abillity0AudioClips[Random.Range(0, abillity0AudioClips.Length)];
        audioSource.Play();
        canUseAbility0 = false;

        travellerFunctions.Ability0(this);
    }

    public virtual void Ability1()
    {
        Debug.Log(name + " : Ability1");
        AD += 300;

        audioSource.clip = abillity1AudioClips[Random.Range(0, abillity1AudioClips.Length)];
        audioSource.Play();
        canUseAbility1 = false;

        travellerFunctions.Ability1(this);
    }

    public virtual void Ability2()
    {
        Debug.Log(name + " : Ability2");
        HP += 300;

        audioSource.clip = abillity2AudioClips[Random.Range(0, abillity2AudioClips.Length)];
        audioSource.Play();
        canUseAbility2 = false;

        travellerFunctions.Ability2(this);
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

    private void Interaction()
    {
        Debug.Log(name + " : Interaction");
        nearInteractiveObject.GetComponent<InteractiveObject>().Interaction();
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
        Debug.Log(isHealthy);
        if (isHealthy == false) return;

        HP -= damage;
        isHealthy = false;

        hpBar.fillAmount = (float)HP / maxHP;
        hpText.text = $"{HP} / {maxHP}";

        bloodingPanel.SetActive(false);
        bloodingPanel.SetActive(true);

        if (HP <= 0) Collapse();
    }

    public void Collapse()
    {
        Debug.Log($"{name} : Collapse");

        playerRB.bodyType = RigidbodyType2D.Static;
        GameManager.Instance.StartCoroutine(GameManager.Instance.GameOver());

        hpBar.fillAmount = 0;
        hpText.text = $"0 / {maxHP}";

        animator.SetTrigger("Collapse");
    }

    public void AcquireExp(int value)
    {
        exp += value;
        if (exp >= requiredExp) LevelUp();

        expBar.fillAmount = (float)exp / requiredExp;
        expText.text = Mathf.Floor((float)exp / requiredExp * 100) + "%";
    }

    [ContextMenu("LevelUp")]
    private void LevelUp()
    {
        level++;
        exp = exp - requiredExp;
        requiredExp += 100; 

        levelText.text = $"Lv. {level}";
        
        ObjectManager.Instance.GetQueue(PoolType.Smoke, transform);
        AbilityManager.Instance.LevelUp();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UpperDoor")) GameManager.Instance.StartCoroutine(GameManager.Instance.MigrateRoom(Vector2.up, 1));
        else if (other.CompareTag("LowerDoor")) GameManager.Instance.StartCoroutine(GameManager.Instance.MigrateRoom(Vector2.down, 0)); 
        else if (other.CompareTag("LeftDoor")) GameManager.Instance.StartCoroutine(GameManager.Instance.MigrateRoom(Vector2.left, 3)); 
        else if (other.CompareTag("RightDoor")) GameManager.Instance.StartCoroutine(GameManager.Instance.MigrateRoom(Vector2.right, 2)); 

        if (other.CompareTag("InteractiveObject"))
        {
            nearInteractiveObject = other.gameObject;
            canInteraction = true;
            interactionIcon.SetActive(true);
            attackIcon.SetActive(false);
        }  
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("InteractiveObject"))
        {
            nearInteractiveObject = other.gameObject;
            canInteraction = true;
            interactionIcon.SetActive(true);
            attackIcon.SetActive(false);
        }  
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InteractiveObject"))
        {
            nearInteractiveObject = other.gameObject;
            canInteraction = false;
            interactionIcon.SetActive(false);
            attackIcon.SetActive(true);
        }     
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("InteractiveObject"))
        {
            nearInteractiveObject = other.gameObject;
            canInteraction = false;
            interactionIcon.SetActive(false);
            attackIcon.SetActive(true);
        }  
    }
}
