using System.Collections;
using UnityEngine;

public class DogSwellfish : NormalMonster
{
    [SerializeField] private GameObject ultWarning;
    [SerializeField] private GameObject ultReadyParticle;
    [SerializeField] private BulletMove[] ultGO;
    private Coroutine attack;
    private bool isAttacking;

    protected override void OnDisable()
    {
        base.OnDisable();
        if (attack != null)
        {
            StopCoroutine(attack);
        }
    }

    private void Move()
    {
        Rigidbody2D.velocity = Random.insideUnitCircle.normalized * MoveSpeed;
        SpriteRenderer.flipX = Rigidbody2D.velocity.x > 0;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        ultReadyParticle.SetActive(true);
        yield return new WaitForSeconds(.1f);
        // Animator.SetBool("ULT", true);
        ultWarning.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 180 + 1)));
        ultWarning.SetActive(true);
        yield return new WaitForSeconds(.6f);
        ultReadyParticle.SetActive(false);

        for (int i = 0; i < ultGO.Length; i++)
        {
            ultGO[i].transform.position = transform.position;
            ultGO[i].transform.rotation = ultWarning.transform.GetChild(i).rotation;
            ultGO[i].SetDirection((ultWarning.transform.GetChild(i).position - transform.position).normalized);
            ultGO[i].gameObject.SetActive(true);
        }

        ultWarning.SetActive(false);
        // Animator.SetBool("ULT", false);
        yield return new WaitForSeconds(.5f);
        isAttacking = false;
    }

    protected override void _ReceiveHit()
    {
        base._ReceiveHit();
        Move();

        if (isAttacking == false)
        {
            attack = StartCoroutine(Attack());
        }
    }
}