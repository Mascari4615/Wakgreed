using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private CinemachineTargetGroup cinemachineTargetGroup;
    public GameObject bloodingPanel;

    private float curCoolDown, curAttackCoolDown;
    private bool isHealthy, canAttack, canInteraction;

    private float h = 0;
    private float v = 0;

    private Rigidbody2D playerRB;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private GameObject target = null;
    private GameObject nearInteractiveObject;
    [SerializeField] private EnemyRunTimeSet EnemyRunTimeSet;
    private float bbolBBolCoolDown = 0.3f;
    private float curBBolBBolCoolDown = 0;

    public int curWeaponNumber = 1;
    public Weapon curWeapon;
    [SerializeField] private Weapon weaponA;
    [SerializeField] private Weapon weaponB;
    [SerializeField] private GameObject hand;

    [SerializeField] private BuffRunTimeSet buffRunTimeSet;
    
    private void Awake()
    {
        instance = this;
        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cinemachineTargetGroup = GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
    }

    private void OnEnable()
    {
        Initialize(true);
    }

    public void Initialize(bool spawnZero)
    {
        if (spawnZero) transform.position = Vector3.zero;
        else transform.position = Vector3.zero + Vector3.up * -47;

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
        canAttack = true;
        canInteraction = false;
        h = 0;
        v = 0;

        playerRB.bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = transform;
        animator.SetTrigger("WakeUp");
        animator.SetBool("Move", false);

        if (weaponPosition.childCount > 0) Destroy(weaponPosition.GetChild(0).gameObject);
        Instantiate(curWeapon.resource, weaponPosition);
        
        AD.RuntimeValue = curWeapon.maxDamage;
        foreach (var weaponBuff in curWeapon.buffs)
        {
            buffRunTimeSet.Add(weaponBuff);
            weaponBuff.hasCondition = true;
        }

        StopCoroutine(DashASD());
        StartCoroutine(DashASD());
        StopCoroutine(UpdateDashStack());
        StartCoroutine(UpdateDashStack());
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);
        spriteRenderer.color = isHealthy == true ? Color.white : new Color(1, 1, 1, (float)100 / 255);
        if (Time.timeScale == 0) return;

        // Targeting();
        CheckCoolDown(ref isHealthy, ref curCoolDown, 1 /*무적 시간*/);
        CheckCoolDown(ref canAttack, ref curAttackCoolDown, 1 / AS.RuntimeValue);

        Move();
        if (Input.GetMouseButton(0) && canAttack) BasicAttack();
        Dash();
        if (Input.GetKeyDown(KeyCode.F) && canInteraction) Interaction();

        if (Input.GetKeyDown(KeyCode.Q) && curWeapon.skillQ != null) { curWeapon.skillQ.Use(); }
        if (Input.GetKeyDown(KeyCode.E) && curWeapon.skillE != null) { curWeapon.skillE.Use(); }

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) SwitchWeapon();
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(2);

        if (Input.GetKeyDown(KeyCode.R) && curWeapon.magazine != 0) StartCoroutine((curWeapon.baseAttack as RangedSkill).Reload(this));
        if (curWeapon.magazine != 0 && curWeapon.ammo == 0) StartCoroutine((curWeapon.baseAttack as RangedSkill).Reload(this));
    }

    public bool isSwitching = false;
    public void SwitchWeapon(int targetWeaponNumber = 0, Weapon targetWeapon = null)
    {
        if (!isSwitching) StartCoroutine(_SwitchWeapon(targetWeaponNumber, targetWeapon));
    }
    private IEnumerator _SwitchWeapon(int targetWeaponNumber = 0, Weapon targetWeapon = null)
    {
        if (!isSwitching)
        {
            if (targetWeaponNumber == 0 || (targetWeaponNumber != 0 && targetWeaponNumber != curWeaponNumber))
            {
                foreach (var weaponBuff in curWeapon.buffs)
                {
                    weaponBuff.hasCondition = false;
                    buffRunTimeSet.Remove(weaponBuff);
                }
                Destroy(weaponPosition.GetChild(0).gameObject);

                isSwitching = true;

                if (targetWeapon == null)
                {
                    if (targetWeaponNumber == 0)
                    {
                        // 스크롤 스위칭
                        if (curWeaponNumber == 1) { curWeaponNumber = 2; curWeapon = weaponB; }
                        else if (curWeaponNumber == 2) { curWeaponNumber = 1; curWeapon = weaponA; }
                    }
                    else
                    {
                        // 넘버 스위칭
                        if (targetWeaponNumber == 1) { curWeaponNumber = 1; curWeapon = weaponA; }
                        else if (targetWeaponNumber == 2) { curWeaponNumber = 2; curWeapon = weaponB; }
                    }
                }
                else
                {
                    if (curWeaponNumber == 1)
                    {
                        weaponA = targetWeapon;
                        curWeapon = weaponA;
                    }
                    else if (curWeaponNumber == 2)
                    {
                        weaponB = targetWeapon;
                        curWeapon = weaponB; ;
                    }
                }

                Instantiate(curWeapon.resource, weaponPosition);
                //weaponPosition.GetChild(0).GetComponent<SpriteRenderer>().sprite = curWeapon.icon;
                AD.RuntimeValue = curWeapon.maxDamage;
                foreach (var weaponBuff in curWeapon.buffs)
                {
                    buffRunTimeSet.Add(weaponBuff);
                    weaponBuff.hasCondition = true;
                }


                yield return new WaitForSeconds(0.25f);
                isSwitching = false;
            }
        }
    }
    
    private Vector3 derectionPos;
    private bool isDashing = false;
    public float dashParametor = 1;
    private float temptime = 0;
    public GameObject dashasdasd;
    public GameObject[] dashStackUIs;
    private int maxDashStack = 5;
    private int curDashStack = 0;
    private float dashCoolTime = 1f;

    private void Dash()
    {
        Debug.DrawRay(transform.position, derectionPos * 0.9f, Color.red);

        if ((Input.GetMouseButtonDown(1) && !isDashing && curDashStack > 0))
        {
            curDashStack--;
            RuntimeManager.PlayOneShot("event:/SFX/Wakgood/Dash", transform.position);
            derectionPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y, 0).normalized;
            dashasdasd.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x) * Mathf.Rad2Deg - 90);
            isDashing = true;
            playerRB.velocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            temptime += 0.02f;
            if (temptime >= 0.1f) { isDashing = false; temptime = 0; }
            foreach (RaycastHit2D hitObject in Physics2D.RaycastAll(transform.position, derectionPos, 0.9f))
            {
                if (hitObject.transform.CompareTag("Wall")) { isDashing = false; temptime = 0; playerRB.velocity = Vector3.zero; }
            }
            if (isDashing == false)
            {
                playerRB.velocity = Vector3.zero;
            }
            else
            {
                // transform.position += derectionPos * Time.deltaTime * 10 * dashParametor;
                playerRB.velocity = derectionPos * 10 * dashParametor;
            }
        }
    }
    private IEnumerator DashASD()
    {
        while (true)
        {
            for (int i = 0; i < maxDashStack; i++)
            {
                if (i < curDashStack)
                {
                    dashStackUIs[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    dashStackUIs[i].transform.GetChild(0).gameObject.SetActive(false);

                }
            }

            
            yield return new WaitForSeconds(0.02f);
        }       
    }

    private IEnumerator UpdateDashStack()
    {
        while (true)
        {
            if (curDashStack < maxDashStack)
            {
                yield return new WaitForSeconds(dashCoolTime);
                curDashStack++;
            }
            else
            {
                yield return new WaitForSeconds(0.02f);
            } 
        }
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

    private List<int> horizontalMoveDirectionList = new();
    private List<int> verticalMoveDirectionList = new();

    private void Move()
    {
        if (Input.GetKey(KeyCode.A)) { if (!horizontalMoveDirectionList.Contains(-1)) horizontalMoveDirectionList.Add(-1); }
        else horizontalMoveDirectionList.Remove(-1);
        if (Input.GetKey(KeyCode.D)) { if (!horizontalMoveDirectionList.Contains(1)) horizontalMoveDirectionList.Add(1); }
        else horizontalMoveDirectionList.Remove(1);
        if (Input.GetKey(KeyCode.W)) { if (!verticalMoveDirectionList.Contains(1)) verticalMoveDirectionList.Add(1); }
        else verticalMoveDirectionList.Remove(1);
        if (Input.GetKey(KeyCode.S)) { if (!verticalMoveDirectionList.Contains(-1)) verticalMoveDirectionList.Add(-1); }
        else verticalMoveDirectionList.Remove(-1); ;

        if (isDashing) return;

        // h = Input.GetAxisRaw("Horizontal");
        // v = Input.GetAxisRaw("Vertical");       

        h = horizontalMoveDirectionList.Count == 0 ? 0 : horizontalMoveDirectionList[horizontalMoveDirectionList.Count - 1];
        v = verticalMoveDirectionList.Count == 0 ? 0 : verticalMoveDirectionList[verticalMoveDirectionList.Count - 1];

        Vector3 moveDirection = new Vector2(h, v).normalized;

        if ((h != 0 || v != 0) && playerRB.bodyType == RigidbodyType2D.Dynamic)
        {
            playerRB.velocity = moveDirection * moveSpeed.RuntimeValue;
            animator.SetBool("Move", true);

            if (curBBolBBolCoolDown > bbolBBolCoolDown)
            {
                ObjectManager.Instance.GetQueue("BBolBBol", transform.position, true);
                curBBolBBolCoolDown = 0;
                bbolBBolCoolDown = Random.Range(0.1f, 0.3f);
            }
            else
            {
                curBBolBBolCoolDown += Time.deltaTime;
            }
        }
        else
        {
            // playerRB.velocity = Vector2.zero;
            animator.SetBool("Move", false);
        }

        attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - (transform.position.y + 0.8f), Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x) * Mathf.Rad2Deg - 90);

        if (target != null)
        {
            //if (target.transform.position.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);
            //else if (target.transform.position.x < transform.position.x) transform.localScale = new Vector3(-1, 1, 1);

            // cinemachineTargetGroup.m_Targets[1].target = target.transform;
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
        }
        else if (target == null)
        {
            if (transform.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
            {
                spriteRenderer.flipX = false;
                //transform.localScale = new Vector3(1, 1, 1);
                weaponPosition.localScale = new Vector3(1, 1, 1);
                weaponPosition.localPosition = new Vector3(.3f, .5f, 0);

                weaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - weaponPosition.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).x - weaponPosition.position.x) * Mathf.Rad2Deg);
            }
            else
            {
                spriteRenderer.flipX = true;
                //transform.localScale = new Vector3(-1, 1, 1);
                weaponPosition.localScale = new Vector3(-1, 1, 1);
                weaponPosition.localPosition = new Vector3(-.3f, .5f, 0);

                weaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(weaponPosition.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y, weaponPosition.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x) * Mathf.Rad2Deg);
            }
        }
        // cinemachineTargetGroup.m_Targets[1].target = null;
    }

    private void Interaction()
    {
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
        canAttack = false;
        curWeapon.baseAttack.Use();
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
        maxHP.RuntimeValue += traveller.growthHP;
        OnHpChange.Raise();

        AD.RuntimeValue += traveller.growthAD;
        AS.RuntimeValue += traveller.growthAS;

        EXP.RuntimeValue -= requiredExp;
        Level.RuntimeValue++;
        requiredExp = (100 * (1 + Level.RuntimeValue));
        OnLevelUp.Raise();
        OnExpChange.Raise();

        ObjectManager.Instance.GetQueue("LevelUpEffect", transform);
        ObjectManager.Instance.GetQueue("DamageText", transform).GetComponent<AnimatedText>().SetText("Level Up!", TextType.Critical);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UpperDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.up, 1));
        else if (other.CompareTag("LowerDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.down, 0));
        else if (other.CompareTag("LeftDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.left, 3));
        else if (other.CompareTag("RightDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.right, 2));

        if (other.CompareTag("NormalArea")) AreaTweener.Instance.AreaToNormal();
        else if (other.CompareTag("Area")) AreaTweener.Instance.NormalToArea(other.transform);

        if (other.CompareTag("InteractiveObject"))
        {
            nearInteractiveObject = other.gameObject;
            canInteraction = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InteractiveObject"))
        {
            canInteraction = false;
        }
    }
}
