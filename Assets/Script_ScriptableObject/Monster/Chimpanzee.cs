using System.Collections;
using UnityEngine;

public class Chimpanzee : NormalMonster
{
    private bool canAttack = false;
    private float nextCoolDown = 0;
    private float attackCoolDown = 5;
    private float castTime = 0.2f;
    private bool isTargeting = false;
    [SerializeField] private GameObject gamja;

    protected override void OnEnable()
    {
        base.OnEnable();
        canAttack = false;
        isTargeting = false;
        castTime = 0.5f;
        nextCoolDown = Time.time + 2;
        gamja.SetActive(false);

        StartCoroutine(Attack());
        StartCoroutine(Targeting());
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (canAttack == false)
            {
                if (nextCoolDown <= Time.time)
                {
                    canAttack = true;
                    gamja.SetActive(false);
                }
            }
            else if (canAttack && isTargeting)
            {
                if (castTime == 0.5f)
                {
                    animator.SetTrigger("ATTACK");
                }

                castTime -= 0.1f;
                if (castTime <= 0)
                {
                    castTime = 0.5f;
                    nextCoolDown = Time.time + attackCoolDown;
                    gamja.transform.localPosition = new Vector3(0, 0, -5);
                    gamja.SetActive(true);
                    canAttack = false;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Targeting()
    {
        while (true)
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
                else if (TravellerController.Instance.transform.position.x < transform.position.x) spriteRenderer.flipX = true;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
