using System.Collections;
using UnityEngine;

public class BuisinessKim : BossMonster
{
    [SerializeField] private GameObject skill0AttackPrefab;
    private BSlash[] skill0AttackGo = new BSlash[2];
    [SerializeField] private LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();

        (skill0AttackGo[0] = Instantiate(skill0AttackPrefab, transform).GetComponent<BSlash>()).gameObject.SetActive(false);
        (skill0AttackGo[1] = Instantiate(skill0AttackPrefab, transform).GetComponent<BSlash>()).gameObject.SetActive(false);
        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            int i = Random.Range(0, 2 + 1);
            switch (i)
            {
                case 0:
                    yield return StartCoroutine(Skill0());
                    break;
                case 1:
                    yield return StartCoroutine(Skill0());
                    break;
                case 2:
                    yield return StartCoroutine(Skill0());
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    protected IEnumerator Skill0()
    {
        yield return new WaitForSeconds(.2f);
        Animator.SetBool("SKILL1", true);

        Vector3 originPos = transform.position;
        Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3(
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f),
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

        Animator.SetTrigger("SKILL1CHARGE");

        SpriteRenderer.flipX = targetPos.x > Wakgood.Instance.transform.position.x;

        for (float j = 0; j <= 1; j += Time.deltaTime * 7)
        {
            Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
            yield return null;
        }

        Vector3 attackDirection = (Wakgood.Instance.transform.position - transform.position).normalized;

        lineRenderer.SetPosition(0, transform.position + (Vector3)Vector2.up);
        lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + attackDirection * 100);
        lineRenderer.gameObject.SetActive(true);
        float angle = Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 1), Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        yield return new WaitForSeconds(.4f);

        Animator.SetTrigger("SKILL1GO");

        skill0AttackGo[0].transform.position = transform.position + (Vector3)Vector2.up;
        skill0AttackGo[0].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        skill0AttackGo[0].SetDirection(attackDirection);
        skill0AttackGo[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(.4f);
        skill0AttackGo[1].transform.position = transform.position + (Vector3)Vector2.up;
        skill0AttackGo[1].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        skill0AttackGo[1].SetDirection(attackDirection);
        skill0AttackGo[1].gameObject.SetActive(true);
        lineRenderer.gameObject.SetActive(false);

        Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(2f);
    }
}
