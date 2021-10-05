using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wakgood : MonoBehaviour, IDamagable
{
    private static Wakgood instance;
    [HideInInspector] public static Wakgood Instance { get { return instance; } }

    public Wakdu traveller;
    public IntVariable maxHP;
    public IntVariable HP;
    public IntVariable AD;
    public FloatVariable AS;
    public IntVariable criticalChance;
    public FloatVariable moveSpeed;
    public IntVariable EXP;
    private int requiredExp;
    public IntVariable Level;
    public GameEvent OnCollapse, OnLevelUp;

    private Transform attackPositionParent;
    public Transform attackPosition { get; private set; }
    public Transform weaponPosition { get; private set; }
    private CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] private GameObject bloodingPanel;

    // private float curAttackCoolDown;
    private bool isHealthy, canAttackA, canAttackB;

    private Rigidbody2D playerRB;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // private GameObject target = null;
    [SerializeField] private EnemyRunTimeSet EnemyRunTimeSet;
    private float bbolBBolCoolDown = 0.3f;
    private float curBBolBBolCoolDown = 0;

    private int curWeaponNumber = 1;
    public Weapon curWeapon { get; private set; }
    [SerializeField] private Weapon weaponA;
    [SerializeField] private Weapon weaponB;
    [SerializeField] private GameObject hand;

    [SerializeField] private BuffRunTimeSet buffRunTimeSet;

    private Camera mainCamera;
    private Vector3 worldMousePoint;

    private List<int> hInputList = new();
    private List<int> vInputList = new();
    private int h;
    private int v;

    private bool isDashing = false;
    [SerializeField] private float dashParametor = 1;
    [SerializeField] private GameObject[] dashStackUIs;
    private int maxDashStack = 5;
    private int curDashStack = 0;
    private float dashCoolTime = 1f;
    private Dictionary<int, InteractiveObject> nearInteractiveObjectDic = new();

    private IEnumerator ChangeWithDelay(bool changeValue, float delay, System.Action<bool> makeResult)
    {
        // 참고 : https://velog.io/@sonohoshi/10.-Unity에서-일정-시간-이후-값을-바꾸는-방법
        yield return new WaitForSeconds(delay);
        makeResult(changeValue);
    }

    private void Awake()
    {
        instance = this;
        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        attackPositionParent = transform.Find("AttackPosParent");
        attackPosition = attackPositionParent.GetChild(0);
        weaponPosition = transform.Find("WeaponPos");
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cinemachineTargetGroup = GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
        mainCamera = Camera.main;

        Initialize(true);
    }

    public void Initialize(bool spawnZero)
    {
        if (spawnZero) transform.position = Vector3.zero;
        else transform.position = Vector3.zero + Vector3.up * -47;

        maxHP.RuntimeValue = traveller.baseHP;
        HP.RuntimeValue = maxHP.RuntimeValue;

        AD.RuntimeValue = traveller.baseAD;
        AS.RuntimeValue = traveller.baseAS;

        criticalChance.RuntimeValue = traveller.baseCriticalChance;
        moveSpeed.RuntimeValue = traveller.baseMoveSpeed;

        Level.RuntimeValue = 0;
        requiredExp = (100 * (1 + Level.RuntimeValue));
        EXP.RuntimeValue = 0;

        isHealthy = true;
        //curAttackCoolDown = 0;
        canAttackA = true;
        canAttackB = true;

        playerRB.bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = transform;
        animator.SetTrigger("WakeUp");
        animator.SetBool("Move", false);

        if (weaponPosition.childCount > 0) Destroy(weaponPosition.GetChild(0).gameObject);
        curWeapon = weaponA;
        Instantiate(curWeapon.resource, weaponPosition);

        AD.RuntimeValue = curWeapon.maxDamage;
        foreach (var weaponBuff in curWeapon.buffs)
        {
            buffRunTimeSet.Add(weaponBuff);
            weaponBuff.hasCondition = true;
        }

        StartCoroutine(DashASD());
        StartCoroutine(UpdateDashStack());
    }

    private void Update()
    {
        worldMousePoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);
        spriteRenderer.color = isHealthy == true ? Color.white : new Color(1, 1, 1, (float)100 / 255);
        if (Time.timeScale == 0) return;

        // Targeting();
        Move();
        if (Input.GetMouseButton(0)) BasicAttack();
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && curDashStack > 0) StartCoroutine(Dash());
        if (Input.GetKeyDown(KeyCode.F) && nearInteractiveObjectDic.Count != 0)
        {
            InteractiveObject nearInteractiveObject = null;
            float distance = float.MaxValue;
            foreach (var item in nearInteractiveObjectDic.Values)
            {
                if (Vector2.Distance(transform.position, item.transform.position) < distance)
                    nearInteractiveObject = item;
            }
            nearInteractiveObject.Interaction();
        }

        if (Input.GetKeyDown(KeyCode.Q) && curWeapon.skillQ != null) { curWeapon.skillQ.Use(); }
        if (Input.GetKeyDown(KeyCode.E) && curWeapon.skillE != null) { curWeapon.skillE.Use(); }

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) SwitchWeapon();
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(2);

        if (Input.GetKeyDown(KeyCode.R) && curWeapon.magazine != 0) StartCoroutine((curWeapon.baseAttack as RangedAttack).Reload(this));
        if (curWeapon.magazine != 0 && curWeapon.ammo == 0) StartCoroutine((curWeapon.baseAttack as RangedAttack).Reload(this));
    }

    public bool IsSwitching { get; set; } = false;

    public void SwitchWeapon(int targetWeaponNumber = 0)
    {
        if (IsSwitching) return;
        IsSwitching = true;

        foreach (var weaponBuff in curWeapon.buffs)
        {
            weaponBuff.hasCondition = false;
            buffRunTimeSet.Remove(weaponBuff);
        }
        Destroy(weaponPosition.GetChild(0).gameObject);

        if (targetWeaponNumber == 0)
        {   // 스크롤 스위칭
            if (curWeaponNumber == 1) { curWeaponNumber = 2; curWeapon = weaponB; }
            else if (curWeaponNumber == 2) { curWeaponNumber = 1; curWeapon = weaponA; }
        }
        else
        {   // 넘버 스위칭
            if (targetWeaponNumber == 1) { curWeaponNumber = 1; curWeapon = weaponA; }
            else if (targetWeaponNumber == 2) { curWeaponNumber = 2; curWeapon = weaponB; }
        }

        Instantiate(curWeapon.resource, weaponPosition);
        AD.RuntimeValue = curWeapon.maxDamage;
        foreach (var weaponBuff in curWeapon.buffs)
        {
            buffRunTimeSet.Add(weaponBuff);
            weaponBuff.hasCondition = true;
        }

        StartCoroutine(ChangeWithDelay(false, .25f, value => IsSwitching = value));
    }

    public void SwitchWeapon(Weapon targetWeapon)
    {
        if (IsSwitching) return;
        IsSwitching = true;

        foreach (var weaponBuff in curWeapon.buffs)
        {
            weaponBuff.hasCondition = false;
            buffRunTimeSet.Remove(weaponBuff);
        }
        Destroy(weaponPosition.GetChild(0).gameObject);

        if (curWeaponNumber == 1) { weaponA = targetWeapon; curWeapon = weaponA; }
        else if (curWeaponNumber == 2) { weaponB = targetWeapon; curWeapon = weaponB; }

        Instantiate(curWeapon.resource, weaponPosition);
        AD.RuntimeValue = curWeapon.maxDamage;
        foreach (var weaponBuff in curWeapon.buffs)
        {
            buffRunTimeSet.Add(weaponBuff);
            weaponBuff.hasCondition = true;
        }

        StartCoroutine(ChangeWithDelay(false, .25f, value => IsSwitching = value));
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        // Vector3 direction = new Vector3(worldMousePoint.x - transform.position.x, worldMousePoint.y - transform.position.y, 0).normalized;
        Vector3 direction = new Vector3(h, v, 0).normalized;
        RuntimeManager.PlayOneShot("event:/SFX/Wakgood/Dash", transform.position);

        curDashStack--;
        playerRB.velocity = Vector3.zero;
        for (float temptime = 0; temptime <= 0.1f; temptime += Time.deltaTime)
        {
            foreach (RaycastHit2D hitObject in Physics2D.RaycastAll(transform.position, direction, 0.9f))
                if (hitObject.transform.gameObject.layer.Equals("Wall")) goto DashEnd;

            // transform.position += direction * Time.deltaTime * 10 * dashParametor;
            playerRB.velocity = direction * 10 * dashParametor;
            yield return new WaitForFixedUpdate();
        }
    DashEnd:
        playerRB.velocity = Vector3.zero;
        isDashing = false;
    }

    private IEnumerator DashASD()
    {
        while (true)
        {
            for (int i = 0; i < maxDashStack; i++)
            {
                if (i < curDashStack) dashStackUIs[i].transform.GetChild(0).gameObject.SetActive(true);
                else dashStackUIs[i].transform.GetChild(0).gameObject.SetActive(false);
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
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.A)) { if (!hInputList.Contains(-1)) hInputList.Add(-1); }
        else if (hInputList.Contains(-1)) hInputList.Remove(-1);
        if (Input.GetKey(KeyCode.D)) { if (!hInputList.Contains(1)) hInputList.Add(1); }
        else if (hInputList.Contains(1)) hInputList.Remove(1);
        if (Input.GetKey(KeyCode.W)) { if (!vInputList.Contains(1)) vInputList.Add(1); }
        else if (vInputList.Contains(1)) vInputList.Remove(1);
        if (Input.GetKey(KeyCode.S)) { if (!vInputList.Contains(-1)) vInputList.Add(-1); }
        else if (vInputList.Contains(-1)) vInputList.Remove(-1); ;

        if (isDashing) return;

        h = hInputList.Count == 0 ? 0 : hInputList[hInputList.Count - 1];
        v = vInputList.Count == 0 ? 0 : vInputList[vInputList.Count - 1];

        if ((h != 0 || v != 0) && playerRB.bodyType.Equals(RigidbodyType2D.Dynamic))
        {
            playerRB.velocity = new Vector2(h, v).normalized * moveSpeed.RuntimeValue;
            animator.SetBool("Move", true);

            if (curBBolBBolCoolDown > bbolBBolCoolDown)
            {
                ObjectManager.Instance.PopObject("BBolBBol", transform, true);
                curBBolBBolCoolDown = 0;
                bbolBBolCoolDown = Random.Range(0.1f, 0.3f);
            }
            else curBBolBBolCoolDown += Time.deltaTime;
        }
        else
        {
            // 테스트
            // playerRB.velocity = Vector2.zero;
            animator.SetBool("Move", false);
        }

        attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(worldMousePoint.y - (transform.position.y + 0.8f), worldMousePoint.x - transform.position.x) * Mathf.Rad2Deg - 90);

        if (transform.position.x < worldMousePoint.x)
        {
            spriteRenderer.flipX = false;
            weaponPosition.localScale = Vector3.one;
            weaponPosition.localPosition = new Vector3(.3f, .5f, 0);
            weaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(worldMousePoint.y - weaponPosition.position.y, worldMousePoint.x - weaponPosition.position.x) * Mathf.Rad2Deg);
        }
        else
        {
            spriteRenderer.flipX = true;
            weaponPosition.localScale = new Vector3(-1, 1, 1);
            weaponPosition.localPosition = new Vector3(-.3f, .5f, 0);
            weaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(weaponPosition.position.y - worldMousePoint.y, weaponPosition.position.x - worldMousePoint.x) * Mathf.Rad2Deg);
        }

        /* if (target != null)
        {
            if (target.transform.position.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);
            else if (target.transform.position.x < transform.position.x) transform.localScale = new Vector3(-1, 1, 1);

            cinemachineTargetGroup.m_Targets[1].target = target.transform;
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
        }
        cinemachineTargetGroup.m_Targets[1].target = null; */
    }

    /*
    private void Targeting()
    {
        target = null;
        float targetDist = 10;
        float currentDist;

        foreach (GameObject monster in EnemyRunTimeSet.Items)
        {
            currentDist = Vector2.Distance(transform.position, monster.transform.position);
            if (currentDist > targetDist) continue;

            foreach (RaycastHit2D hitObject in Physics2D.RaycastAll(transform.position, monster.transform.position - transform.position))
            {
                // if (hitObject.transform.CompareTag("Wall")) break;
                if (hitObject.transform.gameObject.layer.Equals("Wall")) break;
                else if (hitObject.transform.CompareTag("Monster") || hitObject.transform.CompareTag("Boss"))
                {
                    target = monster.gameObject;
                    targetDist = currentDist;
                }
            }
        }
    }*/

    public void BasicAttack()
    {
        if (curWeaponNumber == 1 && !canAttackA) return;
        else if (curWeaponNumber == 2 && !canAttackB) return;

        curWeapon.baseAttack.Use();

        if (curWeaponNumber == 1)
        {
            canAttackA = false;
            StartCoroutine(ChangeWithDelay(true, 1f / curWeapon.attackSpeed, value => canAttackA = value));
        }
        if (curWeaponNumber == 2)
        {
            canAttackB = false;
            StartCoroutine(ChangeWithDelay(true, 1f / curWeapon.attackSpeed, value => canAttackB = value));
        }
    }

    public void ReceiveDamage(int damage)
    {
        if (isHealthy == false) return;

        HP.RuntimeValue -= damage;
        isHealthy = false;
        StartCoroutine(ChangeWithDelay(true, .5f, value => isHealthy = value));

        bloodingPanel.SetActive(false);
        bloodingPanel.SetActive(true);

        if (HP.RuntimeValue <= 0) { StopAllCoroutines(); StartCoroutine(Collapse()); }
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

        AD.RuntimeValue += traveller.growthAD;
        AS.RuntimeValue += traveller.growthAS;

        EXP.RuntimeValue -= requiredExp;
        Level.RuntimeValue++;
        requiredExp = (100 * (1 + Level.RuntimeValue));
        OnLevelUp.Raise();

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);
        ObjectManager.Instance.PopObject("DamageText", transform).GetComponent<AnimatedText>().SetText("Level Up!", TextType.Critical);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UpperDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.up, 1));
        else if (other.CompareTag("LowerDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.down, 0));
        else if (other.CompareTag("LeftDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.left, 3));
        else if (other.CompareTag("RightDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.right, 2));

        else if (other.CompareTag("NormalArea")) AreaTweener.Instance.AreaToNormal();
        else if (other.CompareTag("Area")) AreaTweener.Instance.NormalToArea(other.transform);

        else if (other.CompareTag("InteractiveObject"))
        {
            if (!nearInteractiveObjectDic.ContainsKey(other.GetInstanceID()))
                nearInteractiveObjectDic.Add(other.GetInstanceID(), other.GetComponent<InteractiveObject>());
            else Debug.LogError("ㅈ버그");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("InteractiveObject"))
        {
            if (nearInteractiveObjectDic.ContainsKey(other.GetInstanceID()))
                nearInteractiveObjectDic.Remove(other.GetInstanceID());
            else Debug.LogError("ㅈ버그");
        }
    }
}
