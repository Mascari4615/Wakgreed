using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wakgood : MonoBehaviour, IHitable
{
    private static Wakgood instance;
    [HideInInspector] public static Wakgood Instance { get { return instance; } }

    public Wakdu wakdu;
    [SerializeField] private IntVariable maxHP;
    [SerializeField] private IntVariable HP;
    [SerializeField] private GameEvent OnDamage;
    [SerializeField] private IntVariable AD;
    [SerializeField] private FloatVariable AS;
    [SerializeField] private IntVariable criticalChance;
    [SerializeField] private FloatVariable moveSpeed;
    [SerializeField] private IntVariable EXP;
    private int requiredExp;
    [SerializeField] private IntVariable Level;
    [SerializeField] private GameEvent OnCollapse, OnLevelUp;
    [SerializeField] private FloatVariable Evasion;

    private Transform attackPositionParent;
    public Transform attackPosition { get; private set; }
    public Transform weaponPosition { get; private set; }
    private CinemachineTargetGroup cinemachineTargetGroup;

    private bool isHealthy;

    private Rigidbody2D playerRB;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float bbolBBolCoolDown = 0.3f;
    private float curBBolBBolCoolDown = 0;

    public int curWeaponNumber { get; private set; } = 1;
    public Weapon curWeapon { get; private set; }
    [SerializeField] private Weapon weapon1;
    [SerializeField] private Weapon weapon2;
    [SerializeField] private Weapon hochi;
    [SerializeField] private Weapon hand;
    
    [SerializeField] private BuffRunTimeSet buffRunTimeSet;

    private Vector3 worldMousePoint;

    private List<int> hInputList = new();
    private List<int> vInputList = new();
    private int h;
    private int v;

    private bool isDashing = false;
    [SerializeField] private float dashParametor = 1;
    private int maxDashStack = 5;
    [SerializeField] private IntVariable curDashStack;
    private float dashCoolTime = 1f;
    private Dictionary<int, InteractiveObject> nearInteractiveObjectDic = new();
    public bool IsSwitching { get; set; } = false;

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
        curWeapon = hochi;

        Initialize(true);
    }

    public void Initialize(bool spawnZero)
    {
        StopAllCoroutines();

        if (spawnZero) transform.position = Vector3.zero;
        else transform.position = Vector3.zero + Vector3.up * -47;

        maxHP.RuntimeValue = wakdu.baseHP;
        HP.RuntimeValue = maxHP.RuntimeValue;
        AS.RuntimeValue = wakdu.baseAS;
        criticalChance.RuntimeValue = wakdu.baseCriticalChance;
        moveSpeed.RuntimeValue = wakdu.baseMoveSpeed;
        Level.RuntimeValue = 0;
        requiredExp = (100 * (1 + Level.RuntimeValue));
        EXP.RuntimeValue = 0;
        isHealthy = true;
        IsSwitching = false;

        playerRB.bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = transform;
        animator.SetTrigger("WakeUp");
        animator.SetBool("Move", false);

        if (weaponPosition.childCount > 0) Destroy(weaponPosition.GetChild(0).gameObject);
        curWeapon.OnRemove();

        weapon1 = hochi;
        UIManager.Instance.weapon1Sprite.SetSlot(weapon1);
        UIManager.Instance.weapon1SkillQ.gameObject.SetActive(false);
        if (weapon1.skillQ) UIManager.Instance.weapon1SkillQ.SetSlot(weapon1.skillQ);
        UIManager.Instance.weapon1SkillE.gameObject.SetActive(false);
        if (weapon1.skillE) UIManager.Instance.weapon1SkillE.SetSlot(weapon1.skillE);

        weapon2 = hand;
        UIManager.Instance.weapon2Sprite.SetSlot(weapon2);
        UIManager.Instance.weapon2SkillQ.gameObject.SetActive(false);
        if (weapon2.skillQ) UIManager.Instance.weapon2SkillQ.SetSlot(weapon2.skillQ);
        UIManager.Instance.weapon2SkillE.gameObject.SetActive(false);
        if (weapon2.skillE) UIManager.Instance.weapon2SkillE.SetSlot(weapon2.skillE);

        if (curWeaponNumber != 1)
        {
            curWeaponNumber = 1;
            UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchWeapon());
        }
        curWeapon = weapon1;
        curWeapon.OnEquip();
        Instantiate(curWeapon.resource, weaponPosition);

        AD.RuntimeValue = curWeapon.maxDamage;      
        StartCoroutine(UpdateDashStack());
    }

    private void Update()
    {
        worldMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);
        spriteRenderer.color = isHealthy == true ? Color.white : new Color(1, 1, 1, (float)100 / 255);
        if (Time.timeScale == 0) return;

        Move();
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && curDashStack.RuntimeValue > 0) StartCoroutine(Dash());

        if (Input.GetMouseButton(0)) curWeapon.BaseAttack();
        if (Input.GetKeyDown(KeyCode.Q)) curWeapon.SkillQ();
        if (Input.GetKeyDown(KeyCode.E)) curWeapon.SkillE();
        if (Input.GetKeyDown(KeyCode.R)) curWeapon.Reload();

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) SwitchCurWeapon();
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCurWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCurWeapon(2);

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
    }

    public void SwitchCurWeapon(int targetWeaponNumber = 0)
    {
        if (IsSwitching) return;
        IsSwitching = true;

        UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchWeapon());

        curWeapon.OnRemove();
        Destroy(weaponPosition.GetChild(0).gameObject);

        if (targetWeaponNumber == 0)
        {   // 스크롤 스위칭
            if (curWeaponNumber == 1) { curWeaponNumber = 2; curWeapon = weapon2; }
            else if (curWeaponNumber == 2) { curWeaponNumber = 1; curWeapon = weapon1; }
        }
        else
        {   // 넘버 스위칭
            if (targetWeaponNumber == 1) { curWeaponNumber = 1; curWeapon = weapon1; }
            else if (targetWeaponNumber == 2) { curWeaponNumber = 2; curWeapon = weapon2; }
        }

        Instantiate(curWeapon.resource, weaponPosition);
        curWeapon.OnEquip();

        StartCoroutine(TtmdaclExtension.ChangeWithDelay(false, .25f, value => IsSwitching = value));
    }

    public void SwitchWeapon(Weapon targetWeapon)
    {
        if (IsSwitching) return;
        IsSwitching = true;

        curWeapon.OnRemove();
        Destroy(weaponPosition.GetChild(0).gameObject);

        if (curWeaponNumber == 1)
        {
            weapon1 = targetWeapon;
            curWeapon = weapon1;

            UIManager.Instance.weapon1Sprite.SetSlot(weapon1);
            UIManager.Instance.weapon1SkillQ.gameObject.SetActive(weapon1.skillQ);
            if (weapon1.skillQ) UIManager.Instance.weapon1SkillQ.SetSlot(weapon1.skillQ);
            UIManager.Instance.weapon1SkillE.gameObject.SetActive(weapon1.skillE);
            if (weapon1.skillE) UIManager.Instance.weapon1SkillE.SetSlot(weapon1.skillE);
        }
        else if (curWeaponNumber == 2)
        {
            weapon2 = targetWeapon;
            curWeapon = weapon2;

            UIManager.Instance.weapon2Sprite.SetSlot(weapon2);
            UIManager.Instance.weapon2SkillQ.gameObject.SetActive(weapon2.skillQ);
            if (weapon2.skillQ) UIManager.Instance.weapon2SkillQ.SetSlot(weapon2.skillQ);
            UIManager.Instance.weapon2SkillE.gameObject.SetActive(weapon2.skillE);
            if (weapon2.skillE) UIManager.Instance.weapon2SkillE.SetSlot(weapon2.skillE);
        }

        Instantiate(curWeapon.resource, weaponPosition);
        AD.RuntimeValue = curWeapon.maxDamage;
        curWeapon.OnEquip();

        StartCoroutine(TtmdaclExtension.ChangeWithDelay(false, .25f, value => IsSwitching = value));
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        Vector3 direction = new Vector3(h, v, 0).normalized;
        RuntimeManager.PlayOneShot("event:/SFX/Wakgood/Dash", transform.position);

        curDashStack.RuntimeValue--;
        playerRB.velocity = Vector3.zero;
        for (float temptime = 0; temptime <= 0.1f; temptime += Time.deltaTime)
        {
            if (Physics2D.BoxCast(transform.position, new Vector2(.5f, .5f), 0f, direction, 0.9f, LayerMask.GetMask("Wall")).collider != null) break;

            playerRB.velocity = 10 * dashParametor * direction;
            yield return new WaitForFixedUpdate();
        }
        playerRB.velocity = Vector3.zero;
        isDashing = false;
    }

    private IEnumerator UpdateDashStack()
    {
        while (true)
        {
            if (curDashStack.RuntimeValue < maxDashStack)
            {
                yield return new WaitForSeconds(dashCoolTime);
                curDashStack.RuntimeValue++;
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

        h = hInputList.Count == 0 ? 0 : hInputList[^1];
        v = vInputList.Count == 0 ? 0 : vInputList[^1];

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
    }

    public void ReceiveHit(int damage)
    {
        if (isHealthy)
        {
            if (Evasion.RuntimeValue >= Random.Range(1, 100 + 1))
            {
                RuntimeManager.PlayOneShot($"event:/SFX/Wakgood/Evasion", transform.position);
                ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText("MISS", TextType.Critical);
            }
            else
            {
                RuntimeManager.PlayOneShot($"event:/SFX/Wakgood/Ahya", transform.position);

                HP.RuntimeValue -= damage;
                OnDamage.Raise();

                isHealthy = false;
                StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, .8f, value => isHealthy = value));

                if (HP.RuntimeValue <= 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(Collapse());
                }
            }
        }
    }

    public void ReceiveHeal(int amout)
    {
        if (HP.RuntimeValue != maxHP.RuntimeValue)
        {
            HP.RuntimeValue += amout;
            ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText(amout.ToString(), TextType.Heal);
        }
    }

    private IEnumerator Collapse()
    {
        playerRB.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("Collapse");
        yield return new WaitForSeconds(2f);
        OnCollapse.Raise();
        enabled = false;
    }

    public void CheckCanLevelUp()
    {
        if (EXP.RuntimeValue >= requiredExp) LevelUp();
    }

    private void LevelUp()
    {
        maxHP.RuntimeValue += wakdu.growthHP;

        AD.RuntimeValue += wakdu.growthAD;
        AS.RuntimeValue += wakdu.growthAS;

        EXP.RuntimeValue -= requiredExp;
        Level.RuntimeValue++;
        requiredExp = (100 * (1 + Level.RuntimeValue));
        OnLevelUp.Raise();

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);
        ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText("Level Up!", TextType.Critical);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("UpperDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.up, 1));
        else if (other.CompareTag("LowerDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.down, 0));
        else if (other.CompareTag("LeftDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.left, 3));
        else if (other.CompareTag("RightDoor")) GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(Vector2.right, 2));

        else if (other.CompareTag("AreaDoor")) AreaTweener.Instance.ChangeArea(other.transform);

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
