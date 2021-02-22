using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class TravellerController : MonoBehaviour
{
    private static TravellerController instance;
    [HideInInspector] public static TravellerController Instance { get { return instance; } }

    public Traveller traveller;
    public IntVariable maxHP;
    public IntVariable HP;
    public IntVariable AD;
    public FloatVariable AS;
    public IntVariable criticalChance;
    public FloatVariable moveSpeed;
    public IntVariable EXP;
    private int requiredExp;
    public IntVariable Level;
    public GameEvent OnHpChange, OnCollapse, OnExpChange, OnLevelUp;

    public Transform attackPositionParent, attackPosition, weaponPosition;
    public JoyStick joyStick;
    private CinemachineTargetGroup cinemachineTargetGroup;
    public GameObject bloodingPanel;
    public GameObject interactionIcon = null;
    public GameObject attackIcon = null;
    public EventTrigger attackButtonEventTrigger;

    private float coolDown = 1;
    private float curCoolDown, curAttackCoolDown;
    private bool isHealthy, canAttack, canInteraction;
    private float[] curSkillCoolDown;
    private bool[] canUseSkill;
    private bool isInputtingAttack, isInputtingInteraction;

    [HideInInspector] public float h = 0;
    private float v = 0;

    private Rigidbody2D playerRB;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EventTrigger.Entry attackPointerDown = new EventTrigger.Entry(), attackPointerUp = new EventTrigger.Entry(), attackPointerEnter = new EventTrigger.Entry(), attackPointerExit = new EventTrigger.Entry();

    private GameObject target;
    public ItemInventory ItemInventory;
    private GameObject nearInteractiveObject;
    [SerializeField] private EnemyRunTimeSet EnemyRunTimeSet;
    private float bbolBBolCoolDown = 0.3f;
    private float curBBolBBolCoolDown = 0;
    
    private void Awake()
    {
        // Debug.Log($"{name} : Awake");
        instance = this;
        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cinemachineTargetGroup = GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();

        attackPointerDown.eventID = EventTriggerType.PointerDown;
        // attackPointerEnter.eventID = EventTriggerType.PointerEnter;
        attackPointerUp.eventID = EventTriggerType.PointerUp;
        // attackPointerExit.eventID = EventTriggerType.PointerExit;
        attackPointerDown.callback.AddListener((PointerEventData) => 
        {
            if (canInteraction) Interaction();
            else isInputtingAttack = true; 
        });
        // attackPointerEnter.callback.AddListener((PointerEventData) => {isInputtingattack = true;});
        attackPointerUp.callback.AddListener((PointerEventData) => {isInputtingAttack = false;});
        // attackPointerExit.callback.AddListener((PointerEventData) => {isInputtingattack = false;});

        attackButtonEventTrigger.triggers.Clear();
        attackButtonEventTrigger.triggers.Add(attackPointerDown);
        // attackButtonEventTrigger.triggers.Add(attackPointerEnter);
        attackButtonEventTrigger.triggers.Add(attackPointerUp);
        // attackButtonEventTrigger.triggers.Add(attackPointerExit);
    }

    private void OnEnable()
    {
        // Debug.Log(name + " : OnEnable");
        Initialize();
    }

    public void Initialize()
    {
        // Debug.Log(name + " : Initialize");
        
        transform.position = Vector3.zero;
        maxHP.RuntimeValue = traveller.baseHP;
        HP.RuntimeValue = maxHP.RuntimeValue;
        OnHpChange.Raise();
        
        AD.RuntimeValue = traveller.baseAD;
        AS.RuntimeValue = traveller.baseAS;

        criticalChance.RuntimeValue = traveller.baseCriticalChance;
        moveSpeed.RuntimeValue = traveller.baseMoveSpeed;

        EXP.RuntimeValue = 0;
        Level.RuntimeValue = 0;
        requiredExp = (100 * (1 + Level.RuntimeValue));
        OnExpChange.Raise();
        
        curCoolDown = 0;
        isHealthy = true;
        curAttackCoolDown = 0;
        curSkillCoolDown = new float[] { 0, 0, 0 };
        canAttack = true;
        canUseSkill = new bool[] { false, false, false };
        canInteraction = false;
        isInputtingAttack = false;
        isInputtingInteraction = false;
        h = 0;
        v = 0;
       
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = transform;
        animator.SetTrigger("WakeUp");
        animator.SetBool("Move", false);

        traveller.abilities.Initialize(this);

        StopCoroutine("Update001");
        StartCoroutine("Update001");
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);
        if (Time.timeScale == 0) return;

        Move();
        if ((isInputtingAttack || Input.GetKey(KeyCode.Space)) && canAttack) BasicAttack();
        traveller.abilities._Update(this);
    }

    private IEnumerator Update001()
    {
        // int i = 0;
        while (true)
        {
            // Debug.Log($"{name} : Update001 {++i}");
            if (Time.timeScale == 0)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            Targeting();

            CheckCoolDown(ref isHealthy, ref curCoolDown, coolDown);
            CheckCoolDown(ref canAttack, ref curAttackCoolDown, 1 / AS.RuntimeValue);
            CheckCoolDown(ref canUseSkill[0], ref curSkillCoolDown[0], traveller.skillCoolDown[0]);
            CheckCoolDown(ref canUseSkill[1], ref curSkillCoolDown[1], traveller.skillCoolDown[1]);
            CheckCoolDown(ref canUseSkill[2], ref curSkillCoolDown[2], traveller.skillCoolDown[2]);

            spriteRenderer.color = isHealthy == true ? Color.white : new Color(1, 1, 1, (float)100 / 255);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CheckCoolDown(ref bool coolDownTarget, ref float currentCoolDown, float coolDown)
    {
        if (coolDownTarget == false)
        {
            currentCoolDown += 0.1f;
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

        if (joyStick.isInputting && playerRB.bodyType == RigidbodyType2D.Dynamic)
        {
            playerRB.velocity = moveDirection * moveSpeed.RuntimeValue;      
            animator.SetBool("Move", true); 

            if (curBBolBBolCoolDown > bbolBBolCoolDown)
            {
                ObjectManager.Instance.GetQueue(PoolType.BBolBBol, transform.position + Vector3.down * 0.75f);
                curBBolBBolCoolDown = 0;
            }      
            else
            {
                curBBolBBolCoolDown += Time.deltaTime;
            }
        }
        else
        {
            playerRB.velocity = Vector2.zero;
            animator.SetBool("Move", false);
        }

        if (target != null)
        {
            if (target.transform.position.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);
            else if (target.transform.position.x < transform.position.x) transform.localScale = new Vector3(-1, 1, 1);

            cinemachineTargetGroup.m_Targets[1].target = target.transform;
            attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - target.transform.position.y, transform.position.x - target.transform.position.x) * Mathf.Rad2Deg + 90);
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
        }
        else if (target == null)
        {
            if (h > 0) transform.localScale = new Vector3(1, 1, 1);       
            else if (h < 0) transform.localScale = new Vector3(-1, 1, 1);

            cinemachineTargetGroup.m_Targets[1].target = null;
            attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(joyStick.inputValue.y, joyStick.inputValue.x) * Mathf.Rad2Deg - 90);
        }
    }

    private void Interaction()
    {
        // Debug.Log(name + " : Interaction");
        isInputtingInteraction = false;
        nearInteractiveObject.GetComponent<InteractiveObject>().Interaction();
    }

    private void Targeting()
    {
        target = null;
        float targetDist = 10;
        float currentDist = 0;

        foreach (GameObject monster in EnemyRunTimeSet.Items)
        {
            // 만약 (현재 몬스터와의 거리 > 타겟 몬스터와의 거리) 스킵 
            currentDist = Vector2.Distance(transform.position, monster.transform.position);
            if (currentDist > targetDist) continue;

            // 아니라면 
            foreach (RaycastHit2D hitObject in Physics2D.RaycastAll(transform.position, monster.transform.position - transform.position))
            {
                if (hitObject.transform.CompareTag("Wall")) break;
                else if (hitObject.transform.CompareTag("Monster") || hitObject.transform.CompareTag("Boss"))
                {
                    target = monster.gameObject;
                    targetDist = currentDist;
                }
            }
        }           
    }

    public void BasicAttack()
    {
        // Debug.Log(name + " : Attack");

        SoundManager.Instance.PlayAudioClip(traveller.basicAttackAudioClips[Random.Range(0, traveller.basicAttackAudioClips.Length)]);
        canAttack = false;

        traveller.abilities.BasicAttack(this);
    }
    
    public void Skill(int i)
    {
        if (!canUseSkill[i]) return;
        canUseSkill[i] = false;

        Debug.Log($"{name} : Ability{i}");

        if (i == 0) traveller.abilities.Skill0(this);
        else if (i == 1) traveller.abilities.Skill1(this);
        else if (i == 2) traveller.abilities.Skill2(this);
    }

    public void ReceiveDamage(int damage)
    {
        if (isHealthy == false) return;

        HP.RuntimeValue -= damage;
        OnHpChange.Raise();
        isHealthy = false;

        bloodingPanel.SetActive(false);
        bloodingPanel.SetActive(true);

        if (HP.RuntimeValue <= 0) StartCoroutine(Collapse());
    }

    private IEnumerator Collapse()
    {
        // Debug.Log($"{name} : Collapse");
        playerRB.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("Collapse");

        yield return new WaitForSeconds(2f);
        OnCollapse.Raise();
        this.enabled = false;
    }

    public void CheckCanLevelUp()
    {
        if (EXP.RuntimeValue >= requiredExp) LevelUp();
    }

    private void LevelUp()
    {
        Debug.Log($"{name} : LevelUp");

        maxHP.RuntimeValue += traveller.growthHP;
        OnHpChange.Raise();
        
        AD.RuntimeValue += traveller.growthAD;
        AS.RuntimeValue += traveller.growthAS;

        EXP.RuntimeValue -= requiredExp;
        Level.RuntimeValue++;
        requiredExp = (100 * (1 + Level.RuntimeValue));
        OnLevelUp.Raise();
        OnExpChange.Raise();
        
        ObjectManager.Instance.GetQueue(PoolType.Smoke, transform);
        ObjectManager.Instance.GetQueue(PoolType.AnimatedText, transform).GetComponent<AnimatedText>().SetText("Level Up!", TextType.Critical);
    }

    private void OnTriggerEnter2D(Collider2D other)
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InteractiveObject"))
        {
            canInteraction = false;
            interactionIcon.SetActive(false);
            attackIcon.SetActive(true);
        }     
    }
}
