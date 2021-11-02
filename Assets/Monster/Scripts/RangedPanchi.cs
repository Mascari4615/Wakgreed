using System.Collections;
using UnityEngine;

public abstract class RangedPanchi : Panchi 
{
    [SerializeField] private GameObject gamja;
    [SerializeField] private float attackCoolTime;
    private bool isTargeting;
    private static readonly int attack = Animator.StringToHash("ATTACK");

    protected override void OnEnable()
    {
        base.OnEnable();
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
            Animator.SetTrigger(attack);
            gamja.transform.localPosition = Vector3.zero;
            gamja.SetActive(true);
        }
    }

    private IEnumerator Targeting()
    {
        WaitForSeconds ws02 = new(0.2f);
        while (true)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Wakgood.Instance.transform.position - transform.position, Vector2.Distance(transform.position, Wakgood.Instance.transform.position), LayerMask.NameToLayer("Everything"));
            foreach (RaycastHit2D t in hit)
            {
                if (t.transform.CompareTag("Wall")) { isTargeting = false; break; }
                else if (t.transform.CompareTag("Player")) isTargeting = true;
            }
            if (isTargeting)
            {
                Debug.DrawRay(transform.position, Wakgood.Instance.transform.position - transform.position, Color.green);

                if (Wakgood.Instance.transform.position.x > transform.position.x) SpriteRenderer.flipX = false;
                else if (Wakgood.Instance.transform.position.x < transform.position.x) SpriteRenderer.flipX = true;
            }
            yield return ws02;
        }
    }
}