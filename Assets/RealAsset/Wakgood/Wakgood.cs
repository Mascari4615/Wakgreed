using Cinemachine;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class Wakgood : MonoBehaviour, IHitable
{
    private static Wakgood instance;
    [HideInInspector] public static Wakgood Instance { get { return instance; } }

    [SerializeField] private Wakdu wakdu;
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

    private SpriteRenderer spriteRenderer;
    private WakgoodCollider wakgoodCollider;
    private WakgoodMove wakgoodMove;

    private Vector3 worldMousePoint;

    public int curWeaponNumber { get; private set; } = 1;
    public Weapon curWeapon { get; private set; }
    public Weapon weapon1 {get; private set;}
    public Weapon weapon2 {get; private set;}
    [SerializeField] private Weapon hochi;
    [SerializeField] private Weapon hand;

    [SerializeField] private BuffRunTimeSet buffRunTimeSet;

    public bool IsSwitching { get; private set; } = false;
    public bool IsCollapsed { get; private set; } = false;


    private void Awake()
    {
        instance = this;
        // attackPosition.transform.position = new Vector3(0, attackPosGap, 0);

        attackPositionParent = transform.Find("AttackPosParent");
        attackPosition = attackPositionParent.GetChild(0);
        weaponPosition = transform.Find("WeaponPos");

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
        IsCollapsed = false;

        cinemachineTargetGroup.m_Targets[0].target = transform;

        if (weaponPosition.childCount > 0) Destroy(weaponPosition.GetChild(0).gameObject);
        if (curWeapon != null) curWeapon.OnRemove();

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

        if (Input.GetMouseButton(0)) curWeapon.BaseAttack();
        if (Input.GetKeyDown(KeyCode.Q)) curWeapon.SkillQ();
        if (Input.GetKeyDown(KeyCode.E)) curWeapon.SkillE();
        if (Input.GetKeyDown(KeyCode.R)) curWeapon.Reload();

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) SwitchCurWeapon();
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCurWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCurWeapon(2);

        if (Input.GetKeyDown(KeyCode.F) && wakgoodCollider.NearInteractiveObjectDic.Count != 0)
        {
            InteractiveObject nearInteractiveObject = null;
            float distance = float.MaxValue;
            foreach (var item in wakgoodCollider.NearInteractiveObjectDic.Values)
            {
                if (Vector2.Distance(transform.position, item.transform.position) < distance)
                    nearInteractiveObject = item;
            }
            nearInteractiveObject.Interaction();
        }

        attackPositionParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(worldMousePoint.y - (transform.position.y + 0.8f), worldMousePoint.x - transform.position.x) * Mathf.Rad2Deg - 90);

        if (transform.position.x < worldMousePoint.x)
        {
            weaponPosition.localScale = Vector3.one;
            weaponPosition.localPosition = new Vector3(.3f, .5f, 0);
            weaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(worldMousePoint.y - weaponPosition.position.y, worldMousePoint.x - weaponPosition.position.x) * Mathf.Rad2Deg);
        }
        else
        {
            weaponPosition.localScale = new Vector3(-1, 1, 1);
            weaponPosition.localPosition = new Vector3(-.3f, .5f, 0);
            weaponPosition.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(weaponPosition.position.y - worldMousePoint.y, weaponPosition.position.x - worldMousePoint.x) * Mathf.Rad2Deg);
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
        wakgoodMove.StopAllCoroutines();
        IsCollapsed = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Animator>().SetTrigger("Collapse");
        wakgoodCollider.enabled = false;
        wakgoodMove.enabled = false;
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
}
