using System.Collections;
using UnityEngine;
public class SniperPanchi : NormalMonster
{
    [SerializeField] LineRenderer lineRenderer;

    protected override void OnEnable()
    {
        base.OnEnable();
        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            yield return StartCoroutine(Skill0());
        }
    }

    private IEnumerator Skill0()
    {
        // Animator.SetBool("SKILL1", true);
        // Animator.SetTrigger("SKILL1CHARGE");

        Vector3 diff = Random.insideUnitCircle * 5f;
        Vector3 _attackDirection = (Wakgood.Instance.transform.position + diff - transform.position).normalized;

        lineRenderer.SetPosition(0, transform.position + (Vector3)Vector2.up);
        lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + _attackDirection * 100);
        lineRenderer.gameObject.SetActive(true);

        StartCoroutine(Casting(castingTime));

        for (float i = 0; i <= 1f; i += Time.deltaTime * 0.5f)
        {
            SpriteRenderer.flipX = transform.position.x > Wakgood.Instance.transform.position.x;
            diff = Vector3.Lerp(diff, Vector3.zero, i * 0.1f);
            _attackDirection = (Wakgood.Instance.transform.position + diff - transform.position).normalized;
            lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + _attackDirection * 100);
            yield return null;
        }

        // Animator.SetTrigger("SKILL1GO");
        yield return new WaitForSeconds(1.2f);
        var a = ObjectManager.Instance.PopObject("Suri", transform.position);

        a.transform.position = transform.position + (Vector3)Vector2.up;
        a.GetComponent<BulletMove>().SetDirection(_attackDirection);
        lineRenderer.gameObject.SetActive(false);

        // Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(5f);
    }
}
