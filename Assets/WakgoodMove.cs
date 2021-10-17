using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WakgoodMove : MonoBehaviour
{
    [SerializeField] private FloatVariable moveSpeed;
    [SerializeField] private IntVariable maxDashStack;
    [SerializeField] private IntVariable curDashStack;
    [SerializeField] private FloatVariable dashCoolTime;
    private Rigidbody2D playerRB;
    private Animator animator;
    private List<int> hInputList = new();
    private List<int> vInputList = new();
    private int horizontalInput;
    private int verticalInput;
    private Vector2 moveDirection;
    private bool mbMoving;
    private bool mbDashing;
    private bool mbCanBbolBbol = true;
    private const float DASH_PARAMETOR = 4;

    public void Initialize()
    {
        animator.SetTrigger("WakeUp");
        animator.SetBool("Move", false);
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(UpdateDashStack());
    }

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.timeScale == 0 || Wakgood.Instance.IsCollapsed)
            return;
        if (!mbDashing)
            Move();

        if (Input.GetKeyDown(KeyCode.Space) && !mbDashing && curDashStack.RuntimeValue > 0)
        {
            StartCoroutine(Dash());
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
        else if (vInputList.Contains(-1)) vInputList.Remove(-1);

        horizontalInput = hInputList.Count == 0 ? 0 : hInputList[^1];
        verticalInput = vInputList.Count == 0 ? 0 : vInputList[^1];

        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
        mbMoving = !moveDirection.Equals(Vector2.zero);
        animator.SetBool("Move", mbMoving);
        playerRB.velocity = moveDirection * moveSpeed.RuntimeValue;

        if (mbMoving && mbCanBbolBbol)
        {
            RuntimeManager.PlayOneShot("event:/SFX/Wakgood/BbolBbol");
            ObjectManager.Instance.PopObject("BBolBBol", transform);
            StartCoroutine(TtmdaclExtension.ChangeWithDelay(!(mbCanBbolBbol = false), Random.Range(0.1f, 0.3f), value => mbCanBbolBbol = value));
        }
    }

    private IEnumerator Dash()
    {
        mbDashing = true;
        RuntimeManager.PlayOneShot("event:/SFX/Wakgood/Dash");
        curDashStack.RuntimeValue--;
        for (float temptime = 0; temptime <= 0.1f; temptime += Time.deltaTime)
        {
            if (Physics2D.BoxCast(transform.position, new Vector2(.5f, .5f), 0f, moveDirection, 0.9f, LayerMask.GetMask("Wall")).collider != null)
                break;

            playerRB.velocity = 10 * DASH_PARAMETOR * moveDirection;
            yield return new WaitForFixedUpdate();
        }
        mbDashing = false;
    }

    private IEnumerator UpdateDashStack()
    {
        while (true)
        {
            if (curDashStack.RuntimeValue < maxDashStack.RuntimeValue)
            {
                yield return new WaitForSeconds(dashCoolTime.RuntimeValue);
                curDashStack.RuntimeValue++;
            }

            yield return null;
        }
    }
}
