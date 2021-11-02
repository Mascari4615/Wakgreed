using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Wakgood : MonoBehaviour, IHitable
{
    [HideInInspector] public static Wakgood Instance { get; private set; }

    [SerializeField] private Wakdu wakdu;
    [FormerlySerializedAs("maxHP")] [SerializeField] private IntVariable maxHp;
    [FormerlySerializedAs("HP")] [SerializeField] private IntVariable hp;
    [FormerlySerializedAs("OnDamage")] [SerializeField] private GameEvent onDamage;
    [FormerlySerializedAs("AD")] [SerializeField] private IntVariable ad;
    [FormerlySerializedAs("AS")] [SerializeField] private FloatVariable @as;
    [SerializeField] private IntVariable criticalChance;
    [SerializeField] private FloatVariable moveSpeed;
    [FormerlySerializedAs("EXP")] [SerializeField] private IntVariable exp;
    private int requiredExp;
    [FormerlySerializedAs("Level")] [SerializeField] private IntVariable level;
    [FormerlySerializedAs("OnCollapse")] [SerializeField] private GameEvent onCollapse;
    [FormerlySerializedAs("OnLevelUp")] [SerializeField] private GameEvent onLevelUp;
    [FormerlySerializedAs("Evasion")] [SerializeField] private FloatVariable evasion;

    private Transform attackPositionParent;
    public Transform AttackPosition { get; private set; }
    public Transform WeaponPosition { get; private set; }
    private CinemachineTargetGroup cinemachineTargetGroup;

    private bool isHealthy;

    private SpriteRenderer spriteRenderer;
    private WakgoodCollider wakgoodCollider;
    private WakgoodMove wakgoodMove;

    private Vector3 worldMousePoint;

    private int CurWeaponNumber { get; set; } = 1;
    public Weapon CurWeapon { get; private set; }
    public Weapon Weapon1 {get; private set;}
    public Weapon Weapon2 {get; private set;}
    [SerializeField] private Weapon hochi;
    [SerializeField] private Weapon hand;

    [SerializeField] private BuffRunTimeSet buffRunTimeSet;
    private static readonly int collapse = Animator.StringToHash("Collapse");

    public bool IsSwitching { get; private set; } = false;
    public bool IsCollapsed { get; private set; } = false;


    private void Awake()
    {
        Instance = this;
        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        attackPositionParent = transform.Find("AttackPosParent");
        AttackPosition = attackPositionParent.GetChild(0);
        WeaponPosition = transform.Find("WeaponPos");

        spriteRenderer = GetComponent<SpriteRenderer>();
        wakgoodCollider = transform.GetChild(0).GetComponent<WakgoodCollider>();
        wakgoodMove = GetComponent<WakgoodMove>();
        cinemachineTargetGroup = GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        StopAllCoroutines();

        transform.position = Vector3.zero;

        maxHp.RuntimeValue = wakdu.baseHp;
        hp.RuntimeValue = maxHp.RuntimeValue;
        @as.RuntimeValue = wakdu.baseAs;
        criticalChance.RuntimeValue = wakdu.baseCriticalChance;
        moveSpeed.RuntimeValue = wakdu.baseMoveSpeed;
        level.RuntimeValue = 0;
        requiredExp = (100 * (1 + level.RuntimeValue));
        exp.RuntimeValue = 0;
        isHealthy = true;
        IsSwitching = false;
        IsCollapsed = false;

        cinemachineTargetGroup.m_Targets[0].target = transform;

        if (WeaponPosition.childCount > 0) Destroy(WeaponPosition.GetChild(0).gameObject);
        if (CurWeapon != null) CurWeapon.OnRemove();

        Weapon1 = hochi;
        UIManager.Instance.weapon1Sprite.SetSlot(Weapon1);
        UIManager.Instance.weapon1SkillQ.gameObject.SetActive(false);
        if (Weapon1.skillQ) UIManager.Instance.weapon1SkillQ.SetSlot(Weapon1.skillQ);
        UIManager.Instance.weapon1SkillE.gameObject.SetActive(false);
        if (Weapon1.skillE) UIManager.Instance.weapon1SkillE.SetSlot(Weapon1.skillE);

        Weapon2 = hand;
        UIManager.Instance.weapon2Sprite.SetSlot(Weapon2);
        UIManager.Instance.weapon2SkillQ.gameObject.SetActive(false);
        if (Weapon2.skillQ) UIManager.Instance.weapon2SkillQ.SetSlot(Weapon2.skillQ);
        UIManager.Instance.weapon2SkillE.gameObject.SetActive(false);
        if (Weapon2.skillE) UIManager.Instance.weapon2SkillE.SetSlot(Weapon2.skillE);

        if (CurWeaponNumber != 1)
        {
            CurWeaponNumber = 1;
            UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchWeapon());
        }
        CurWeapon = Weapon1;
        CurWeapon.OnEquip();
        Instantiate(CurWeapon.resource, WeaponPosition);

        ad.RuntimeValue = CurWeapon.maxDamage;

        wakgoodCollider.enabled = true;
        wakgoodMove.enabled = true;
        wakgoodMove.StopAllCoroutines();
        wakgoodMove.Initialize();
    }

    private void Update()
    {
        if (Time.timeScale == 0 || IsCollapsed) return;

        spriteRenderer.color = isHealthy == true ? Color.white : new Color(1, 1, 1, (float)100 / 255);
        spriteRenderer.flipX = transform.position.x > worldMousePoint.x;
        worldMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0)) CurWeapon.BaseAttack();
        if (Input.GetKeyDown(KeyCode.Q)) CurWeapon.SkillQ();
        if (Input.GetKeyDown(KeyCode.E)) CurWeapon.SkillE();
        if (Input.GetKeyDown(KeyCode.R)) CurWeapon.Reload();

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) SwitchCurWeapon();
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCurWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCurWeapon(2);

        if (Input.GetKeyDown(KeyCode.F) && wakgoodCollider.NearInteractiveObjectDic.Count != 0)
        {
            InteractiveObject nearInteractiveObject = null;
            float distance = float.MaxValue;
            foreach (InteractiveObject item in wakgoodCollider.NearInteractiveObjectDic.Values.Where(item => Vector2.Distance(transform.position, item.transform.position) < distance))
            {
                nearInteractiveObject = item;
                distance = Vector2.Distance(transform.position, item.transform.position);
            }
            nearInteractiveObject.Interaction();
        }

        attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(worldMousePoint.y - (transform.position.y + 0.8f), worldMousePoint.x - transform.position.x) * Mathf.Rad2Deg - 90);

        if (transform.position.x < worldMousePoint.x)
        {
            WeaponPosition.localScale = Vector3.one;
            WeaponPosition.localPosition = new Vector3(.3f, .5f, 0);
            WeaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(worldMousePoint.y - WeaponPosition.position.y, worldMousePoint.x - WeaponPosition.position.x) * Mathf.Rad2Deg);
        }
        else
        {
            WeaponPosition.localScale = new Vector3(-1, 1, 1);
            WeaponPosition.localPosition = new Vector3(-.3f, .5f, 0);
            WeaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(WeaponPosition.position.y - worldMousePoint.y, WeaponPosition.position.x - worldMousePoint.x) * Mathf.Rad2Deg);
        }
    }

    private void SwitchCurWeapon(int targetWeaponNumber = 0)
    {
        if (IsSwitching) return;
        IsSwitching = true;

        UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchWeapon());

        CurWeapon.OnRemove();
        Destroy(WeaponPosition.GetChild(0).gameObject);

        switch (targetWeaponNumber)
        {
            // 스크롤 스위칭
            case 0 when CurWeaponNumber == 1:
                CurWeaponNumber = 2; CurWeapon = Weapon2;
                break;
            case 0:
                {
                    if (CurWeaponNumber == 2) { CurWeaponNumber = 1; CurWeapon = Weapon1; }

                    break;
                }
            // 넘버 스위칭
            case 1:
                CurWeaponNumber = 1; CurWeapon = Weapon1;
                break;
            case 2:
                CurWeaponNumber = 2; CurWeapon = Weapon2;
                break;
        }

        Instantiate(CurWeapon.resource, WeaponPosition);
        CurWeapon.OnEquip();

        StartCoroutine(TtmdaclExtension.ChangeWithDelay(false, .25f, value => IsSwitching = value));
    }

    public void SwitchWeapon(Weapon targetWeapon)
    {
        if (IsSwitching) return;
        IsSwitching = true;

        CurWeapon.OnRemove();
        Destroy(WeaponPosition.GetChild(0).gameObject);

        switch (CurWeaponNumber)
        {
            case 1:
                {
                    Weapon1 = targetWeapon;
                    CurWeapon = Weapon1;

                    UIManager.Instance.weapon1Sprite.SetSlot(Weapon1);
                    UIManager.Instance.weapon1SkillQ.gameObject.SetActive(Weapon1.skillQ);
                    if (Weapon1.skillQ) UIManager.Instance.weapon1SkillQ.SetSlot(Weapon1.skillQ);
                    UIManager.Instance.weapon1SkillE.gameObject.SetActive(Weapon1.skillE);
                    if (Weapon1.skillE) UIManager.Instance.weapon1SkillE.SetSlot(Weapon1.skillE);
                    break;
                }
            case 2:
                {
                    Weapon2 = targetWeapon;
                    CurWeapon = Weapon2;

                    UIManager.Instance.weapon2Sprite.SetSlot(Weapon2);
                    UIManager.Instance.weapon2SkillQ.gameObject.SetActive(Weapon2.skillQ);
                    if (Weapon2.skillQ) UIManager.Instance.weapon2SkillQ.SetSlot(Weapon2.skillQ);
                    UIManager.Instance.weapon2SkillE.gameObject.SetActive(Weapon2.skillE);
                    if (Weapon2.skillE) UIManager.Instance.weapon2SkillE.SetSlot(Weapon2.skillE);
                    break;
                }
        }

        Instantiate(CurWeapon.resource, WeaponPosition);
        ad.RuntimeValue = CurWeapon.maxDamage;
        CurWeapon.OnEquip();

        StartCoroutine(TtmdaclExtension.ChangeWithDelay(false, .25f, value => IsSwitching = value));
    }

    public void ReceiveHit(int damage)
    {
        if (!isHealthy)
        {
            return;
        }

        if (evasion.RuntimeValue >= Random.Range(1, 100 + 1))
        {
            RuntimeManager.PlayOneShot($"event:/SFX/Wakgood/Evasion", transform.position);
            ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText("MISS", TextType.Critical);
        }
        else
        {
            RuntimeManager.PlayOneShot($"event:/SFX/Wakgood/Ahya", transform.position);

            hp.RuntimeValue -= damage;
            onDamage.Raise();

            isHealthy = false;
            StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, .8f, value => isHealthy = value));

            if (hp.RuntimeValue > 0)
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(Collapse());
        }
    }

    public void ReceiveHeal(int amount)
    {
        if (hp.RuntimeValue == maxHp.RuntimeValue)
        {
            return;
        }

        hp.RuntimeValue += amount;
        ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText(amount.ToString(), TextType.Heal);
    }

    private IEnumerator Collapse()
    {
        wakgoodMove.StopAllCoroutines();
        IsCollapsed = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Animator>().SetTrigger(collapse);
        wakgoodCollider.enabled = false;
        wakgoodMove.enabled = false;
        yield return new WaitForSeconds(2f);
        onCollapse.Raise();
        enabled = false;
    }

    public void CheckCanLevelUp()
    {
        if (exp.RuntimeValue >= requiredExp) LevelUp();
    }

    private void LevelUp()
    {
        maxHp.RuntimeValue += wakdu.growthHp;

        ad.RuntimeValue += wakdu.growthAD;
        @as.RuntimeValue += wakdu.growthAs;

        exp.RuntimeValue -= requiredExp;
        level.RuntimeValue++;
        requiredExp = (100 * (1 + level.RuntimeValue));
        onLevelUp.Raise();

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);
        ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText("Level Up!", TextType.Critical);
    }
}