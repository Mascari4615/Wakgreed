using UnityEngine;

public class Slime2 : Monster
{
    private bool canAttack = false;
    private float nextCoolDown = 0;
    private float attackCoolDown = 5;
    private float castTime = 0.2f;
    private bool isTargeting = false;
    [SerializeField] private GameObject slime;

    protected override void OnEnable()
    {
        base.OnEnable();
        canAttack = false;
        isTargeting = false;
        castTime = 0.5f;
        nextCoolDown = Time.time + 2;
        slime.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        Attack();
        Targeting();
    }

    private void Attack()
    {
        if (canAttack == false)
        {
            if (nextCoolDown <= Time.time)
            {
                canAttack = true;
                slime.SetActive(false);
            }
        }
        else if (canAttack && isTargeting)
        {
            if (castTime == 0.5f)
            {
                animator.SetTrigger("Attack");
            }

            castTime -= Time.deltaTime;
            if (castTime <= 0)
            {
                castTime = 0.5f;
                nextCoolDown = Time.time + attackCoolDown;
                slime.transform.localPosition = new Vector3(0, 0, -5);
                slime.SetActive(true);
                canAttack = false;
            }
        }
    }

    private void Targeting()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, TravellerController.Instance.transform.position - transform.position, Vector2.Distance(transform.position, TravellerController.Instance.transform.position), LayerMask.NameToLayer("Everything"));
        for (int j = 0; j < hit.Length; j++)
        {
            if (hit[j].transform.CompareTag("Wall"))
            {
                isTargeting = false;
                break;
            }
            else if (hit[j].transform.CompareTag("Player"))
            {                     
                isTargeting = true;
            }
        }
 
        if (isTargeting)
        {
            Debug.DrawRay(transform.position, TravellerController.Instance.transform.position - transform.position, Color.green);

            if (TravellerController.Instance.transform.position.x > transform.position.x) spriteRenderer.flipX = false;
            else if  (TravellerController.Instance.transform.position.x < transform.position.x) spriteRenderer.flipX = true;
        }
    }
}
