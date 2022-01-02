using Cinemachine;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

[RequireComponent(typeof(WakgoodMove))]
public class Wakgood : MonoBehaviour, IHitable
{
    public static Wakgood Instance { get; private set; }
    public IntVariable criticalChance;
    public IntVariable criticalDamagePer;
    public Wakdu wakdu;
    public IntVariable exp, level;
    public IntVariable hpCur;
    [SerializeField] private IntVariable defence;
    [SerializeField] private IntVariable staticDefence;
    [SerializeField] private MaxHp hpMax;
    [SerializeField] private IntVariable powerInt;
    public TotalPower totalPower;
    public GameEvent useBaseAttack;
    public IntVariable miss, reloadSpeed, bonusAmmo, skillCollBonus;
    public FloatVariable attackSpeed;
    [SerializeField] private FloatVariable moveSpeed, evasion;
    [SerializeField] private BoolVariable canEvasionOnDash;
    [SerializeField] private GameEvent onDamage, onCollapse, onLevelUp;
    public IntVariable BossDamage;
    public IntVariable MobDamage;
    private Transform attackPositionParent;
    public Transform AttackPosition { get; private set; }
    public Transform WeaponPosition { get; private set; }
    private CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] private IntVariable moveSpeedBonus;
    [SerializeField] private IntVariable maxDashStack;
    [SerializeField] private IntVariable curDashStack;
    [SerializeField] private FloatVariable dashCoolTime;
    [SerializeField] private FloatVariable dashChargeSpeed;
    [SerializeField] private GameObject iceObject;
    [SerializeField] private GameObject[] keyObject;
    public bool mbMoving;
    private bool mbCanBbolBbol = true;
    [SerializeField] private float DashParameter = 20;
    public bool isHealthy;

    private SpriteRenderer spriteRenderer;
    public WakgoodCollider wakgoodCollider;
    public WakgoodMove WakgoodMove { get; private set; }

    public Vector2 worldMousePoint;
    public int CurWeaponNumber { get; private set; } = 0;
    public Weapon[] Weapon { get; } = new Weapon[2];
    [SerializeField] private Weapon hochi, hand;

    private static readonly int collapse = Animator.StringToHash("Collapse");

    public bool IsSwitching;
    public bool IsCollapsed;
    [SerializeField] private BoolVariable isFocusOnSomething;

    private GameObject chat;
    private TextMeshProUGUI chatText;
    private Transform statellite;

    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    private Animator animator;

    float gravity;
    Vector3 velocity;
    float velocityXSmoothing;

    public Animator Animator => animator ??= GetComponent<Animator>();
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float gravityP = 1;

    float maxJumpVelocity;
    float minJumpVelocity;

    WakgoodMove wakgoodMove;

    Vector2 directionalInput;

    private void Awake()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        Instance = this;
        hpMax.RuntimeValue = wakdu.baseHp;

        animator = GetComponent<Animator>();
        attackPositionParent = transform.Find("AttackPosParent");
        AttackPosition = attackPositionParent.GetChild(0);
        WeaponPosition = transform.Find("WeaponPos");
        statellite = transform.Find("SatelliteParent");
        spriteRenderer = GetComponent<SpriteRenderer>();
        wakgoodCollider = GetComponent<WakgoodCollider>();
        WakgoodMove = GetComponent<WakgoodMove>();
        cinemachineTargetGroup = GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();

        chat = transform.Find("Canvas").Find("Chat").gameObject;
        chatText = chat.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        wakgoodMove = GetComponent<WakgoodMove>();

    }
    public bool MbDashing;

    private void OnEnable() => Initialize();

    public void Initialize()
    {
        StopAllCoroutines();
        MbDashing = false;

        StartCoroutine(UpdateDashStack());
        transform.position = Vector3.zero;
        Animator.SetTrigger("WakeUp");
        Animator.SetBool("Move", false);
        hpMax.RuntimeValue = wakdu.baseHp;
        hpCur.RuntimeValue = hpMax.RuntimeValue;
        powerInt.RuntimeValue = wakdu.basePower;
        attackSpeed.RuntimeValue = wakdu.baseAttackSpeed;
        moveSpeed.RuntimeValue = wakdu.baseMoveSpeed;
        level.RuntimeValue = 1;
        exp.RuntimeValue = 0;
        isHealthy = true;
        IsSwitching = false;
        IsCollapsed = false;

        cinemachineTargetGroup.m_Targets[0].target = transform;

        if (WeaponPosition.childCount > 0)
        {
            for (int i = 0; i < WeaponPosition.childCount; i++)
            {
                Destroy(WeaponPosition.GetChild(0).gameObject);
            }
        }
        if (Weapon[CurWeaponNumber] != null) Weapon[CurWeaponNumber].OnRemove();

        UIManager.Instance.SetWeaponUI(0, Weapon[0] = hochi);
        UIManager.Instance.SetWeaponUI(1, Weapon[1] = hand);

        foreach (Transform child in statellite)
        {
            Destroy(child.gameObject);
        }

        if (CurWeaponNumber != 0)
        {
            CurWeaponNumber = 0;
            UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchWeapon());
        }

        Weapon[CurWeaponNumber].OnEquip();
        Instantiate(Weapon[CurWeaponNumber].resource, WeaponPosition);

        wakgoodCollider.enabled = true;
        WakgoodMove.enabled = true;
    }

    private bool mbIced;
    private Coroutine iced;
    private Coroutine inputKey;
    public void TryIced()
    {
        if (iced != null) StopCoroutine(iced);
        if (inputKey != null) StopCoroutine(inputKey);
        keyObject[0].SetActive(false);
        keyObject[1].SetActive(false);
        keyObject[2].SetActive(false);
        keyObject[3].SetActive(false);

        iced = StartCoroutine(Iced());
    }
    private IEnumerator Dash()
    {
        MbDashing = true;
        RuntimeManager.PlayOneShot("event:/SFX/Wakgood/Dash");
        curDashStack.RuntimeValue--;
        velocity = ((Vector3)worldMousePoint - transform.position).normalized * DashParameter;
        yield return new WaitForSeconds(.01f);
        MbDashing = false;
    }

    private IEnumerator UpdateDashStack()
    {
        while (true)
        {
            if (curDashStack.RuntimeValue < maxDashStack.RuntimeValue)
            {
                yield return new WaitForSeconds(dashCoolTime.RuntimeValue * (1 - dashChargeSpeed.RuntimeValue / 100));
                curDashStack.RuntimeValue++;
            }

            yield return null;
        }
    }
    private IEnumerator Iced()
    {
        iceObject.SetActive(true);

        for (int i = 0; i < 2; i++)
        {
            int temp = UnityEngine.Random.Range(0, 3 + 1);

            switch (temp)
            {
                case 0:
                    inputKey = StartCoroutine(InputKey(KeyCode.W, 0));
                    break;
                case 1:
                    yield return inputKey = StartCoroutine(InputKey(KeyCode.A, 1));
                    break;
                case 2:
                    yield return inputKey = StartCoroutine(InputKey(KeyCode.S, 2));
                    break;
                case 3:
                    yield return inputKey = StartCoroutine(InputKey(KeyCode.D, 3));
                    break;
            }
        }

        iceObject.SetActive(false);
    }

    private IEnumerator InputKey(KeyCode keyCode, int i)
    {
        keyObject[i].SetActive(true);
        do yield return null;
        while (!Input.GetKeyDown(keyCode));
        keyObject[i].SetActive(false);
    }


    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed.RuntimeValue;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (wakgoodMove.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime * gravityP;
    }

    private void Move()
    {
        directionalInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wakgoodMove.collisions.below)
            {
                if (wakgoodMove.collisions.slidingDownMaxSlope)
                {
                    if (directionalInput.x != -Mathf.Sign(wakgoodMove.collisions.slopeNormal.x))
                    { // not jumping against max slope
                        velocity.y = maxJumpVelocity * wakgoodMove.collisions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * wakgoodMove.collisions.slopeNormal.x;
                    }
                }
                else
                {
                    velocity.y = maxJumpVelocity;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

        CalculateVelocity();
        wakgoodMove.Move(velocity * Time.deltaTime, directionalInput);

        if (wakgoodMove.collisions.above || wakgoodMove.collisions.below)
        {
            if (wakgoodMove.collisions.slidingDownMaxSlope)
            {
                velocity.y += wakgoodMove.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }

        if (Input.GetMouseButtonDown(1) && !MbDashing && curDashStack.RuntimeValue > 0)
            StartCoroutine(Dash());
    }

    private void Update()
    {
        if (IsCollapsed)
        {
            return;
        }

        if (Time.timeScale == 0 || isFocusOnSomething.RuntimeValue || mbIced)
        {
            mbMoving = false;
            Animator.SetBool("Move", mbMoving);
            return;
        }

        Move();

        mbMoving = directionalInput.x != 0;

        Animator.SetBool("Move", mbMoving);
        Animator.SetBool("FLOAT", !WakgoodMove.collisions.below);

        spriteRenderer.color = isHealthy ? Color.white : new Color(1, 1, 1, (float)100 / 255);
        if (isFocusOnSomething.RuntimeValue) return;

        spriteRenderer.flipX = transform.position.x > worldMousePoint.x;
        worldMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) Weapon[CurWeaponNumber].BaseAttack();
        if (Input.GetKeyDown(KeyCode.Q)) Weapon[CurWeaponNumber].SkillQ();
        if (Input.GetKeyDown(KeyCode.E)) Weapon[CurWeaponNumber].SkillE();
        if (Input.GetKeyDown(KeyCode.R)) Weapon[CurWeaponNumber].Reload();
        if (Input.GetKeyDown(KeyCode.F)) wakgoodCollider.GetNearestInteractiveObject()?.Interaction();

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0) SwitchWeapon(CurWeaponNumber == 0 ? 1 : 0);
        /*else if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);*/

        attackPositionParent.transform.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(worldMousePoint.y - (transform.position.y + 0.8f), worldMousePoint.x - transform.position.x) *
            Mathf.Rad2Deg - 90);

        if (transform.position.x < worldMousePoint.x)
        {
            WeaponPosition.localScale = Vector3.one;
            WeaponPosition.localPosition = new Vector3(.3f, .5f, 0);
            WeaponPosition.rotation = Quaternion.Euler(0, 0,
                Mathf.Atan2(worldMousePoint.y - WeaponPosition.position.y,
                    worldMousePoint.x - WeaponPosition.position.x) * Mathf.Rad2Deg);
        }
        else
        {
            WeaponPosition.localScale = new Vector3(-1, 1, 1);
            WeaponPosition.localPosition = new Vector3(-.3f, .5f, 0);
            WeaponPosition.rotation = Quaternion.Euler(0, 0,
                Mathf.Atan2(WeaponPosition.position.y - worldMousePoint.y,
                    WeaponPosition.position.x - worldMousePoint.x) * Mathf.Rad2Deg);
        }

        BBolBBol();
    }

    private void BBolBBol()
    {
        if (!mbMoving || !mbCanBbolBbol || MbDashing)
            return;

        RuntimeManager.PlayOneShot("event:/SFX/Wakgood/BbolBbol");
        ObjectManager.Instance.PopObject("BBolBBol", transform.position);
        StartCoroutine(TtmdaclExtension.ChangeWithDelay(!(mbCanBbolBbol = false), UnityEngine.Random.Range(0.1f, 0.3f), value => mbCanBbolBbol = value));
    }

    public void SwitchWeapon(int targetWeaponNum, Weapon targetWeapon = null)
    {
        if (IsSwitching) return;
        IsSwitching = true;
        StartCoroutine(TtmdaclExtension.ChangeWithDelay(false, .3f, value => IsSwitching = value));

        if (targetWeapon == null)
        {
            Weapon[CurWeaponNumber].OnRemove();
            for (int i = 0; i < WeaponPosition.childCount; i++)
            {
                Destroy(WeaponPosition.GetChild(0).gameObject);
            }
            CurWeaponNumber = targetWeaponNum;

            Instantiate(Weapon[CurWeaponNumber].resource, WeaponPosition);
            Weapon[CurWeaponNumber].OnEquip();

            UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchWeapon());
        }
        else
        {
            if (CurWeaponNumber != targetWeaponNum)
                Weapon[targetWeaponNum] = targetWeapon;
            else
            {
                Weapon[CurWeaponNumber].OnRemove();
                for (int i = 0; i < WeaponPosition.childCount; i++)
                {
                    Destroy(WeaponPosition.GetChild(0).gameObject);
                }
                Weapon[CurWeaponNumber] = targetWeapon;

                Instantiate(Weapon[CurWeaponNumber].resource, WeaponPosition);
                Weapon[CurWeaponNumber].OnEquip();
            }

            UIManager.Instance.SetWeaponUI(targetWeaponNum, targetWeapon);
        }
    }

    public void SwitchWeaponStatic(int targetWeaponNum, Weapon targetWeapon)
    {
        if (CurWeaponNumber != targetWeaponNum)
        {
            Weapon[targetWeaponNum] = targetWeapon;
        }
        else
        {
            for (int i = 0; i < WeaponPosition.childCount; i++)
            {
                Destroy(WeaponPosition.GetChild(0).gameObject);
            }
            Weapon[CurWeaponNumber] = targetWeapon;
            Instantiate(Weapon[CurWeaponNumber].resource, WeaponPosition);
            Weapon[CurWeaponNumber].OnEquip();
        }

        UIManager.Instance.SetWeaponUI(targetWeaponNum, targetWeapon);
    }

    public void ReceiveHit(int damage, HitType hitType = HitType.Normal)
    {
        if (IsCollapsed || !isHealthy || (MbDashing && canEvasionOnDash.RuntimeValue))
            return;

        if (evasion.RuntimeValue >= UnityEngine.Random.Range(1, 100 + 1))
        {
            RuntimeManager.PlayOneShot($"event:/SFX/Wakgood/Evasion", transform.position);
            ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>()
                .SetText("회피!", Color.yellow);
        }
        else
        {
            damage -= staticDefence.RuntimeValue;

            if (damage <= 0)
            {
                RuntimeManager.PlayOneShot($"event:/SFX/Wakgood/Evasion", transform.position);
                ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>()
                    .SetText("무시!", Color.blue);
                return;
            }

            damage = (int)Math.Round(damage * (1 - (float)defence.RuntimeValue / 100), MidpointRounding.AwayFromZero);

            if (damage <= 0)
            {
                RuntimeManager.PlayOneShot($"event:/SFX/Wakgood/Evasion", transform.position);
                ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>()
                    .SetText("무시!", Color.blue);
                return;
            }
            RuntimeManager.PlayOneShot($"event:/SFX/Wakgood/Ahya", transform.position);
            onDamage.Raise();

            if ((hpCur.RuntimeValue -= damage) > 0)
            {
                isHealthy = false;
                StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, .3f, value => isHealthy = value));
            }
            else
            {
                hpCur.RuntimeValue = 0;

                if (GameManager.Instance.isRealBossing.RuntimeValue)
                {
                    if (GameManager.Instance.isRealBossFirstDeath)
                    {
                        FakeCollapse();
                    }
                    else
                    {
                        Collapse();
                    }
                }
                else
                {
                    Collapse();
                }
            }
        }
    }

    public void ReceiveHeal(int amount)
    {
        hpCur.RuntimeValue = Mathf.Clamp(hpCur.RuntimeValue + amount, 0, hpMax.RuntimeValue);
        ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText(amount.ToString(), Color.green);
    }

    public void FakeCollapse()
    {
        IsCollapsed = true;

        Animator.SetTrigger(collapse);
        WakgoodMove.enabled = false;
        wakgoodCollider.enabled = false;

        GameManager.Instance.StartCoroutine(GameManager.Instance.FakeEnding());
    }

    public void Collapse()
    {
        DataManager.Instance.CurGameData.deathCount++;

        StopAllCoroutines();
        WakgoodMove.StopAllCoroutines();
        ObjectManager.Instance.PopObject("Zeolite", transform);
        Animator.SetBool(collapse, true);

        IsCollapsed = true;

        Animator.SetTrigger(collapse);
        WakgoodMove.enabled = false;
        wakgoodCollider.enabled = false;

        GameManager.Instance.StartCoroutine(GameManager.Instance._GameOverAndRecall());
        onCollapse.Raise();
        enabled = false;
    }

    public void CheckCanLevelUp()
    {
        if (exp.RuntimeValue >= 300 * level.RuntimeValue) LevelUp();
    }

    private void LevelUp()
    {
        hpMax.RuntimeValue += wakdu.growthHp;
        powerInt.RuntimeValue += wakdu.growthPower;
        attackSpeed.RuntimeValue += wakdu.growthAttackSpeed;

        exp.RuntimeValue -= 300 * level.RuntimeValue;
        level.RuntimeValue++;
        onLevelUp.Raise();

        ObjectManager.Instance.PopObject("LevelUpEffect", transform);
        ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText("Level Up!", Color.yellow);
    }

    public void SetRigidBodyType(RigidbodyType2D rigidbodyType2D)
    {
        // WakgoodMove.PlayerRb.bodyType = rigidbodyType2D;
    }

    public IEnumerator ShowChat(string msg)
    {
        chatText.text = msg;
        chat.SetActive(true);
        yield return new WaitForSeconds(3f);
        chat.SetActive(false);
    }
}