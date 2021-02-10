using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

public class TravellerController : MonoBehaviour
{
    private static TravellerController instance;
    [HideInInspector] public static TravellerController Instance { get { return instance; } }

    public Traveller traveller;

    public IntVariable maxHP;
    public IntVariable HP;
    public GameEvent OnHpChange;
    public IntVariable AD;
    public FloatVariable AS;
    public IntVariable criticalChance;
    public FloatVariable moveSpeed;
    public IntVariable EXP;
    public GameEvent OnExpChange;
    private int requiredExp;
    public IntVariable LV;
    public GameEvent OnLevelUp;

    public Transform attackPositionParent;
    public Transform attackPosition;
    public Transform weaponPosition;
    public JoyStick joyStick;
    public CinemachineTargetGroup cinemachineTargetGroup;
    public GameObject bloodingPanel;
    public GameObject interactionIcon = null;
    public GameObject attackIcon = null;
    public EventTrigger attackButtonEventTrigger;

    private float coolDown = 1;
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
    private EventTrigger.Entry attackPointerDown = new EventTrigger.Entry();
    private EventTrigger.Entry attackPointerEnter = new EventTrigger.Entry();
    private EventTrigger.Entry attackPointerUp = new EventTrigger.Entry();
    private EventTrigger.Entry attackPointerExit = new EventTrigger.Entry();

    private GameObject target;
    public Inventory inventory;
    private GameObject nearInteractiveObject;
    private AudioSource audioSource;
    [SerializeField] private Traveller[] travellers;
    private TravellerFunctions travellerFunctions;
    [SerializeField] private EnemyRunTimeSet monsters;

    [ContextMenu("Test")]
    public void Test()
    {
        int i = Random.Range(0, inventory.Items.Count);
        inventory.Items[i].OnRemove();
        inventory.Remove(inventory.Items[i]);
    }

    public void ChangeTraveller(int index)
    {
        traveller = travellers[index];
        Initialize();
    }

    private void Awake()
    {
        Debug.Log($"{name} : Awake");
        instance = this;
        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
    }

    private void OnEnable()
    {
        Debug.Log(name + " : OnEnable");
        Initialize();
    }

    private void Initialize()
    {
        Debug.Log(name + " : Initialize");
        travellerFunctions = traveller.travellerFunctions;
        travellerFunctions.Initialize(this);
        
        transform.position = Vector3.zero;
        maxHP.RuntimeValue = traveller.baseHP;
        HP.RuntimeValue = maxHP.RuntimeValue;
        OnHpChange.Raise();
        
        AD.RuntimeValue = traveller.baseAD;
        AS.RuntimeValue = traveller.baseAS;

        criticalChance.RuntimeValue = traveller.baseCriticalChance;
        moveSpeed.RuntimeValue = traveller.baseMoveSpeed;

        EXP.RuntimeValue = 0;
        LV.RuntimeValue = 0;
        requiredExp = (100 * 1 + LV.RuntimeValue);
        OnExpChange.Raise();
        
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

        inventory.Items.Clear();

        attackButtonEventTrigger.triggers.Clear();
        attackButtonEventTrigger.triggers.Add(attackPointerDown);
        // attackButtonEventTrigger.triggers.Add(attackPointerEnter);
        attackButtonEventTrigger.triggers.Add(attackPointerUp);
        // attackButtonEventTrigger.triggers.Add(attackPointerExit);

        travellerFunctions.Initialize(this);
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        Move();
        Targeting();

        CheckCoolDown(ref isHealthy, ref currentCoolDown, coolDown);
        CheckCoolDown(ref canAttack, ref currentAttackCoolDown, 1 / AS.RuntimeValue);
        CheckCoolDown(ref canUseAbility0, ref currentSkill0CoolDown, traveller.abillity0CoolDown);
        CheckCoolDown(ref canUseAbility1, ref currentSkill1CoolDown, traveller.abillity1CoolDown);
        CheckCoolDown(ref canUseAbility2, ref currentSkill2CoolDown, traveller.abillity2CoolDown);

        if (isHealthy == true) spriteRenderer.color = Color.white;
        else if (isHealthy == false) spriteRenderer.color = new Color(1, 1, 1, (float)100 / 255);

        if ((isInputtingAttack || Input.GetKey(KeyCode.Space)) && canAttack) BasicAttack();

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

    private void Move()
    {
        h = joyStick.inputValue.x;
        v = joyStick.inputValue.y;
        Vector3 moveDirection = new Vector2(h, v).normalized;

        if (joyStick.isDraging == true)
        {
            playerRB.velocity = moveDirection * moveSpeed.RuntimeValue;      
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
        isInputtingInteraction = false;
        nearInteractiveObject.GetComponent<InteractiveObject>().Interaction();
    }

    private void Targeting()
    {
        target = null;
        float targetDist = 10;
        float currentDist = 0;
        
        if (monsters.Items.Count > 0)
        {
            foreach (var monster in monsters.Items)
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

    public void BasicAttack()
    {
        Debug.Log(name + " : Attack");

        audioSource.clip = traveller.basicAttackAudioClips[Random.Range(0, traveller.basicAttackAudioClips.Length)];
        audioSource.Play();
        canAttack = false;

        travellerFunctions.BasicAttack(this);
    }

    public void Ability0()
    {
        if (!canUseAbility0) return;

        Debug.Log(name + " : Ability0");
        HP.RuntimeValue += 300;

        audioSource.clip = traveller.abillity0AudioClips[Random.Range(0, traveller.abillity0AudioClips.Length)];
        audioSource.Play();
        canUseAbility0 = false;

        travellerFunctions.Ability0(this);
    }

    public void Ability1()
    {
        if (!canUseAbility1) return;

        Debug.Log(name + " : Ability1");
        AD.RuntimeValue += 300;

        audioSource.clip = traveller.abillity1AudioClips[Random.Range(0, traveller.abillity1AudioClips.Length)];
        audioSource.Play();
        canUseAbility1 = false;

        travellerFunctions.Ability1(this);
    }

    public void Ability2()
    {
        if (!canUseAbility2) return;

        Debug.Log(name + " : Ability2");
        HP.RuntimeValue += 300;

        audioSource.clip = traveller.abillity2AudioClips[Random.Range(0, traveller.abillity2AudioClips.Length)];
        audioSource.Play();
        canUseAbility2 = false;

        travellerFunctions.Ability2(this);
    }

    public void ReceiveDamage(int damage)
    {
        if (isHealthy == false) return;

        SetHP(-damage);
        isHealthy = false;

        bloodingPanel.SetActive(false);
        bloodingPanel.SetActive(true);

        if (HP.RuntimeValue <= 0) Collapse();
    }

    private void Collapse()
    {
        Debug.Log($"{name} : Collapse");

        playerRB.bodyType = RigidbodyType2D.Static;
        GameManager.Instance.StartCoroutine(GameManager.Instance.GameOver());

        animator.SetTrigger("Collapse");
        this.enabled = false;
    }

    private void SetHP(int value)
    {
        HP.RuntimeValue += value;
        OnHpChange.Raise();
    }

    public void CheckCanLevelUp()
    {
        if (EXP.RuntimeValue >= requiredExp) LevelUp();
    }

    private void LevelUp()
    {
        OnLevelUp.Raise();
        LV.RuntimeValue++;
        SetExp(-requiredExp);
        requiredExp = (100 * 1 + LV.RuntimeValue); 
        
        ObjectManager.Instance.GetQueue(PoolType.Smoke, transform);
    }

    private void SetExp(int value)
    {
        EXP.RuntimeValue += value;
        OnExpChange.Raise();
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
