using System.Collections;
using UnityEngine;

public class BabyPanchi : NormalMonster
{
    [SerializeField] private BulletMove bullet;
    [SerializeField] private float attackCoolTime;
    private Coroutine attack;
    private float curAttackCoolTime;
    private Vector3 direction;

    protected override void OnEnable()
    {
        base.OnEnable();

        bullet.gameObject.SetActive(false);
        bullet.transform.localPosition = Vector3.zero;

        curAttackCoolTime = attackCoolTime / 2;
        attack = StartCoroutine(Attack());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (attack != null)
        {
            StopCoroutine(attack);
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (curAttackCoolTime > 0)
            {
                SpriteRenderer.flipX = IsWakgoodRight();
                curAttackCoolTime -= 0.1f;
                yield return ws01;
            }
            else
            {
                direction = GetDirection();
                yield return StartCoroutine(Casting(castingTime));
                Animator.SetTrigger("ATTACK");
                yield return ws1;
                curAttackCoolTime = attackCoolTime;
            }
        }
    }

    public void GamjaOn()
    {
        bullet.transform.localPosition = Vector3.zero;
        bullet.SetDirection(direction);
        bullet.gameObject.SetActive(true);
    }
}