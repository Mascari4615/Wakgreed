using System.Collections;
using UnityEngine;

public class Hikiking : BossMonster
{
    protected override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(Attack());
        UIManager.Instance.bossHpBar.SetActive(true);
    }

    protected override void Update()
    {
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);

        UIManager.Instance.redParent.transform.localScale = new Vector3(Mathf.Lerp(UIManager.Instance.redParent.transform.localScale.x, (float)HP / maxHP, Time.deltaTime * 30f), 1, 1);
    }

    private IEnumerator Attack()
    {
        float t = 0;
        float attackCoolTime = 3;

        while (true)
        {
            t += Time.deltaTime;

            if (t >= attackCoolTime)
            {
                t = 0;

                // switch (Random.Range(0, 3))
                switch (2)
                {
                    case 0:
                        yield return new WaitForSeconds(1f);
                        break;

                    case 1:
                        yield return new WaitForSeconds(1f);
                        break;

                    case 2:
                        Transform originTransform = rigidbody2D.transform;
                        for (int i = 0; i < 5; i++)
                        {
                            rigidbody2D.transform.position = originTransform.position + new Vector3(((i+2)%2==1?1:-1) * Random.Range(10f, 20f), (-1 + Random.Range(0, 2) * 2) * Random.Range(10f, 20f));
                            yield return new WaitForSeconds(0.2f);
                        }
                        yield return new WaitForSeconds(0.5f);
                        rigidbody2D.transform.position = TravellerController.Instance.transform.position;

                        yield return new WaitForSeconds(1f);
                        break;
                }
            }
            else
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }      
        }
    }

    public override void ReceiveDamage(int damage, TextType damageType = TextType.Normal)
    {
        base.ReceiveDamage(damage, damageType);
    }

    protected override IEnumerator Collapse()
    {
        return base.Collapse();
    }

    private void OnDisable()
    {
        UIManager.Instance.bossHpBar.SetActive(false);
    }
}
