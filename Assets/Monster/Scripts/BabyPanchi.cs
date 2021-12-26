using System.Collections;
using UnityEngine;

public class BabyPanchi : NormalMonster
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float attackCoolTime;
    private bool isTargeting;

    protected override void OnEnable()
    {
        base.OnEnable();
        isTargeting = false;

        bullet.SetActive(false);
        bullet.transform.localPosition = Vector3.zero;

        StartCoroutine(Attack());
        StartCoroutine(Targeting());
    }

    private IEnumerator Attack()
    {
        WaitForSeconds wsCool = new(attackCoolTime - 0.7f);
        WaitForSeconds ws02 = new(0.2f);

        while (true)
        {
            while (!isTargeting) yield return ws02;

            yield return wsCool;
            yield return StartCoroutine(Casting(0.7f));
            Animator.SetTrigger("ATTACK");
        }
    }

    public void GamjaOn()
    {
        bullet.transform.localPosition = Vector3.zero;
        bullet.SetActive(true);
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
                SpriteRenderer.flipX = Wakgood.Instance.transform.position.x > transform.position.x;
            }

            yield return ws02;
        }
    }
}
