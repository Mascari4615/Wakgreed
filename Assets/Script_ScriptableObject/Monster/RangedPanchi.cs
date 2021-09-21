using System.Collections;
using UnityEngine;

public abstract class RangedPanchi : Panchi 
{
    [SerializeField] private GameObject gamja;
    [SerializeField] private float attackCoolTime;
    private bool isTargeting = false;

    protected override void OnEnable()
    {
        base.OnEnable(); ;
        isTargeting = false;
        gamja.SetActive(false);
        gamja.transform.localPosition = Vector3.zero;

        StartCoroutine(Attack());
        StartCoroutine(Targeting());
    }

    private IEnumerator Attack()
    {
        WaitForSeconds wsCool = new(attackCoolTime);
        WaitForSeconds ws01 = new(0.1f);
        while (true)
        {
            yield return wsCool;
            while (!isTargeting) yield return ws01;
            animator.SetTrigger("ATTACK");
            gamja.transform.localPosition = Vector3.zero;
            gamja.SetActive(true);
        }
    }

    private IEnumerator Targeting()
    {
        WaitForSeconds ws02 = new(0.2f);
        while (true)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, TravellerController.Instance.transform.position - transform.position, Vector2.Distance(transform.position, TravellerController.Instance.transform.position), LayerMask.NameToLayer("Everything"));
            for (int j = 0; j < hit.Length; j++)
            {
                if (hit[j].transform.CompareTag("Wall")) { isTargeting = false; break; }
                else if (hit[j].transform.CompareTag("Player")) isTargeting = true;
            }
            if (isTargeting)
            {
                Debug.DrawRay(transform.position, TravellerController.Instance.transform.position - transform.position, Color.green);

                if (TravellerController.Instance.transform.position.x > transform.position.x) spriteRenderer.flipX = false;
                else if (TravellerController.Instance.transform.position.x < transform.position.x) spriteRenderer.flipX = true;
            }
            yield return ws02;
        }
    }
}