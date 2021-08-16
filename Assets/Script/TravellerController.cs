using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private CinemachineTargetGroup cinemachineTargetGroup;
    public GameObject bloodingPanel;

    private float coolDown = 1;
    private float curCoolDown, curAttackCoolDown;
    private bool isHealthy, canAttack, canInteraction;
    private float[] curSkillCoolDown;
    private bool[] canUseSkill;

    [HideInInspector] public float h = 0;
    private float v = 0;

    private Rigidbody2D playerRB;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private GameObject target = null;
    public ItemInventory ItemInventory;
    private GameObject nearInteractiveObject;
    [SerializeField] private EnemyRunTimeSet EnemyRunTimeSet;
    private float bbolBBolCoolDown = 0.3f;
    private float curBBolBBolCoolDown = 0;

    private int curWeaponNumber = 1;
    public Weapon curWeapon;
    [SerializeField] private Weapon weaponA;
    [SerializeField] private Weapon weaponB;
    [SerializeField] private GameObject hand;

    [SerializeField] private BuffRunTimeSet buffRunTimeSet;
    [SerializeField] private DataBuffer<Buff> buffBuffer;

    private void Awake()
    {
        // Debug.Log($"{name} : Awake");
        instance = this;
        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cinemachineTargetGroup = GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
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
        h = 0;
        v = 0;
       
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = transform;
        animator.SetTrigger("WakeUp");
        animator.SetBool("Move", false);

        traveller.abilities.Initialize(this);

        Instantiate(curWeapon.resource, weaponPosition);
        //weaponPosition.GetChild(0).GetComponent<SpriteRenderer>().sprite = curWeapon.icon;
        AD.RuntimeValue = curWeapon.maxDamage;
        foreach (var weaponBuff in curWeapon.buffs)
        {
            buffRunTimeSet.Add(weaponBuff);
            weaponBuff.hasCondition = true;
        }

        StopCoroutine("Update001");
        StartCoroutine("Update001");
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);
        if (Time.timeScale == 0) return;

        Move();
        if (Input.GetMouseButton(0) && canAttack) BasicAttack();
        Dash();
        if (Input.GetKeyDown(KeyCode.F) && canInteraction) Interaction();
        traveller.abilities._Update(this);
 
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) StartCoroutine(SwitchWeapon());
        else if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(SwitchWeapon(1));
        else if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(SwitchWeapon(2));

        if (Input.GetKeyDown(KeyCode.R) && curWeapon.magazine != 0) StartCoroutine((curWeapon.baseAttack as RangedSkill).Reload(this));
        if (curWeapon.magazine != 0 && curWeapon.ammo == 0) StartCoroutine((curWeapon.baseAttack as RangedSkill).Reload(this));

        //weaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - weaponPosition.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).x - weaponPosition.position.x) * Mathf.Rad2Deg);
    }

    public bool isSwitching = false;
    /// <summary>
    /// 왁굳이 현재 들고 있는 무기 스위칭
    /// </summary>
    /// <param name="targetWeapon"> 현재 들고 있는 무기에서 스위칭 할 무기</param>
    /// <param name="isExternalSwitching"> False - 내부 스위칭, True - 외부 스위칭</param>
    /// <returns></returns>
    public IEnumerator SwitchWeapon(int targetWeaponNumber = 0, Weapon targetWeapon = null)
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

    private void FixedUpdate()
    {
        //Debug.Log(Time.fixedDeltaTime);
        if (isDashing)
        {
            temptime += Time.fixedDeltaTime;
            if (temptime >= 0.1f) { isDashing = false; temptime = 0; }
            foreach (RaycastHit2D hitObject in Physics2D.RaycastAll(transform.position, derectionPos, 0.9f))
            {
                if (hitObject.transform.CompareTag("Wall")) { isDashing = false; temptime = 0; playerRB.velocity = Vector3.zero; }
            }
            if (isDashing == false)
            {
                playerRB.velocity = Vector3.zero;
                return;
            }
                // transform.position += derectionPos * Time.deltaTime * 10 * dashParametor;
                playerRB.velocity = derectionPos * 10 * dashParametor;
            //Debug.Log("Dashing");
        }
    }
    Vector3 derectionPos;
    bool isDashing = false;
    public float dashParametor = 1;
    float temptime = 0;
    public GameObject dashasdasd;

    private void Dash()
    {
        Debug.DrawRay(transform.position, derectionPos * 0.9f, Color.red);
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) && !isDashing) 
        {
            // derectionPos = Input.GetKey(KeyCode.LeftShift) ? new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y, 0).normalized : new Vector3(h, v, 0);
            derectionPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y, 0).normalized;
            dashasdasd.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x) * Mathf.Rad2Deg - 90);
            isDashing = true;
            playerRB.velocity = Vector3.zero;
            //Debug.Log("Dashed");
        }   
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

            // Targeting();

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

    private List<int> horizontalMoveDirectionList = new List<int>();
    private List<int> verticalMoveDirectionList = new List<int>();

    private void Move()
    {
        if (Input.GetKey(KeyCode.A)) { if (!horizontalMoveDirectionList.Contains(-1)) horizontalMoveDirectionList.Add(-1); }
        else horizontalMoveDirectionList.Remove(-1);
        if (Input.GetKey(KeyCode.D)) { if (!horizontalMoveDirectionList.Contains(1)) horizontalMoveDirectionList.Add(1); }
        else horizontalMoveDirectionList.Remove(1);
        if (Input.GetKey(KeyCode.W)) { if (!verticalMoveDirectionList.Contains(1)) verticalMoveDirectionList.Add(1); }
        else verticalMoveDirectionList.Remove(1);
        if (Input.GetKey(KeyCode.S)) { if (!verticalMoveDirectionList.Contains(-1)) verticalMoveDirectionList.Add(-1); }
        else verticalMoveDirectionList.Remove(-1);;

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
                //ObjectManager.Instance.GetQueue(PoolType.BBolBBol, transform.position);
                ObjectManager.Instance.GetQueue("BBolBBol", transform.position);
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

        attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - (transform.position.y+0.8f), Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x) * Mathf.Rad2Deg - 90);

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
        // Debug.Log(name + " : Interaction");
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
        //Debug.Log(name + " : Attack");
        //SoundManager.Instance.PlayAudioClip(traveller.basicAttackAudioClips[Random.Range(0, traveller.basicAttackAudioClips.Length)]);
        canAttack = false;
        // traveller.abilities.BasicAttack(this);
        curWeapon.baseAttack.Attack(this);
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
        
        //ObjectManager.Instance.GetQueue(PoolType.Smoke, transform);
        ObjectManager.Instance.GetQueue("LevelUpEffect", transform);
        //ObjectManager.Instance.GetQueue(PoolType.AnimatedText, transform).GetComponent<AnimatedText>().SetText("Level Up!", TextType.Critical);
        ObjectManager.Instance.GetQueue("DamageText", transform).GetComponent<AnimatedText>().SetText("Level Up!", TextType.Critical);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UpperDoor")) GameManager.Instance.StartCoroutine(GameManager.Instance.MigrateRoom(Vector2.up, 1));
        else if (other.CompareTag("LowerDoor")) GameManager.Instance.StartCoroutine(GameManager.Instance.MigrateRoom(Vector2.down, 0)); 
        else if (other.CompareTag("LeftDoor")) GameManager.Instance.StartCoroutine(GameManager.Instance.MigrateRoom(Vector2.left, 3)); 
        else if (other.CompareTag("RightDoor")) GameManager.Instance.StartCoroutine(GameManager.Instance.MigrateRoom(Vector2.right, 2));

        if (other.CompareTag("NormalArea")) AreaTweener.Instance.TweenArea(Area.Normal);
        else if (other.CompareTag("TestArea")) AreaTweener.Instance.TweenArea(Area.Test);

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
