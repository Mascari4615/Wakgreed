using System.Collections;
using UnityEngine;

public class FirePanchi : NormalMonster
{
    private bool bIsAttacking = false;
    private static readonly WaitForSeconds ws6 = new(6);
    [SerializeField] private GameObject circle;
    [SerializeField] private GameObject warning;

    protected override void OnEnable()
    {
        base.OnEnable();
        warning.SetActive(false);
        circle.SetActive(false);
        bIsAttacking = false;

        StartCoroutine(UpdateFlip());
        StartCoroutine(Attack());
    }

    private IEnumerator UpdateFlip()
    {
        while (true)
        {
            if (!bIsAttacking)
                SpriteRenderer.flipX = transform.position.x < Wakgood.Instance.transform.position.x;
            yield return ws01;
        }
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            bIsAttacking = true;

            Vector3 randomPos = Wakgood.Instance.transform.position + (Vector3) Random.insideUnitCircle;
            circle.transform.position = randomPos;
            warning.transform.position = randomPos;
            Animator.SetTrigger("READY");
            warning.SetActive(true);
            yield return StartCoroutine(Casting(1.5f));
            Animator.SetTrigger("GO");
            yield return new WaitForSeconds(1f);
            warning.SetActive(false);
            circle.SetActive(true);
            yield return ws6;
            Animator.SetTrigger("OFF");
            yield return new WaitForSeconds(1f);
            circle.SetActive(false);

            bIsAttacking = false;

            yield return ws6;
        }
    }
}
