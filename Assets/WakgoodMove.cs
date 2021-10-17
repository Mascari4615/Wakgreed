using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WakgoodMove : MonoBehaviour
{
    [SerializeField] private FloatVariable moveSpeed;

    private Vector3 worldMousePoint;

    private bool isDashing = false;
    [SerializeField] private float dashParametor = 1;
    private int maxDashStack = 5;
    [SerializeField] private IntVariable curDashStack;
    private float dashCoolTime = 1f;

    private float bbolBBolCoolDown = 0.3f;
    private float curBBolBBolCoolDown = 0;

    private Rigidbody2D playerRB;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private List<int> hInputList = new();
    private List<int> vInputList = new();
    private int h;
    private int v;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize()
    {
        animator.SetTrigger("WakeUp");
        animator.SetBool("Move", false);
        playerRB.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(UpdateDashStack());
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        worldMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Move();
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && curDashStack.RuntimeValue > 0) StartCoroutine(Dash());
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
            // Å×½ºÆ®
            // playerRB.velocity = Vector2.zero;
            animator.SetBool("Move", false);
        }

        if (transform.position.x < worldMousePoint.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
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
}
