using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;

public class WakgoodMove : MonoBehaviour
{
    public Rigidbody2D PlayerRb => playerRb ??= GetComponent<Rigidbody2D>();
    public Animator Animator => animator ??= GetComponent<Animator>();
    public bool MbDashing { get; private set; }

    [SerializeField] private FloatVariable moveSpeed;
    [SerializeField] private IntVariable moveSpeedBonus;
    [SerializeField] private IntVariable maxDashStack;
    [SerializeField] private IntVariable curDashStack;
    [SerializeField] private FloatVariable dashCoolTime;
    [SerializeField] private FloatVariable dashChargeSpeed;
    [SerializeField] private BoolVariable isFocusOnSomething;
    private Rigidbody2D playerRb;
    private Animator animator;
    private readonly List<int> hInputList = new();
    private readonly List<int> vInputList = new();
    private int horizontalInput;
    private int verticalInput;
    private Vector2 moveDirection;
    private bool mbMoving;
    private bool mbCanBbolBbol = true;
    private static readonly int wakeUp = Animator.StringToHash("WakeUp");
    private static readonly int move = Animator.StringToHash("Move");
    private static readonly int collapse = Animator.StringToHash("Collapse");
    private const float DashParameter = 4;

    public void Initialize()
    {
        Animator.SetTrigger(wakeUp);
        Animator.SetBool(move, false);
        PlayerRb.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(UpdateDashStack());
    }

    private void Update()
    {
        if (Wakgood.Instance.IsCollapsed)
        {
            Animator.SetBool(collapse, true);
            return;
        }

        if (Time.timeScale == 0 || isFocusOnSomething.RuntimeValue)
        {
            mbMoving = false;
            Animator.SetBool(move, mbMoving);
            return;
        }

        if (!MbDashing)
            Move();

        if (Input.GetKeyDown(KeyCode.Space) && mbMoving && !MbDashing && curDashStack.RuntimeValue > 0)
            StartCoroutine(Dash());
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
        else if (vInputList.Contains(-1)) vInputList.Remove(-1);

        horizontalInput = hInputList.Count == 0 ? 0 : hInputList[^1];
        verticalInput = vInputList.Count == 0 ? 0 : vInputList[^1];

        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
        mbMoving = !moveDirection.Equals(Vector2.zero);
        Animator.SetBool(move, mbMoving);
        PlayerRb.velocity = moveDirection * (float)Math.Round(moveSpeed.RuntimeValue * (1 - (float)moveSpeedBonus.RuntimeValue / 100), MidpointRounding.AwayFromZero);
        if (!mbMoving || !mbCanBbolBbol)
            return;

        RuntimeManager.PlayOneShot("event:/SFX/Wakgood/BbolBbol");
        ObjectManager.Instance.PopObject("BBolBBol", transform.position);
        StartCoroutine(TtmdaclExtension.ChangeWithDelay(!(mbCanBbolBbol = false), UnityEngine.Random.Range(0.1f, 0.3f), value => mbCanBbolBbol = value));
    }

    private IEnumerator Dash()
    {
        MbDashing = true;
        RuntimeManager.PlayOneShot("event:/SFX/Wakgood/Dash");
        curDashStack.RuntimeValue--;
        for (float temptime = 0; temptime <= 0.1f; temptime += Time.deltaTime)
        {
            if (Physics2D.BoxCast(transform.position, new Vector2(.5f, .5f), 0f, moveDirection, 0.9f, LayerMask.GetMask("Wall")).collider != null)
                break;

            PlayerRb.velocity = 10 * DashParameter * moveDirection;
            yield return new WaitForFixedUpdate();
        }
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
}
