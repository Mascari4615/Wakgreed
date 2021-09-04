using System.Collections;
using UnityEngine;

public class Hikiking : BossMonster
{
    [SerializeField] private GameObject baseattack;

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
        
        int ultStack = 0;

        while (true)
        {
            if (t >= attackCoolTime)
            {
                Debug.Log("Attack");
                t = 0;

                int temp = Random.Range(0, 2);

                if (temp == 0)
                {
                    Debug.Log("0 Start");
                    int attackcount = Random.Range(1, 4);

                    for (int i = 0; i < attackcount; i++)
                    {
                        Vector3 originpos = rigidbody2D.transform.position;
                        Vector3 targetpos = TravellerController.Instance.transform.position + new Vector3((-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f), (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

                        for (float j = 0; j <= 1; j += 0.02f * 5)
                        {
                            rigidbody2D.transform.position = Vector3.Lerp(originpos, targetpos, j);
                            yield return new WaitForSeconds(0.02f);
                        }

                        yield return new WaitForSeconds(0.4f);

                        Instantiate(baseattack, TravellerController.Instance.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 180f))));
                        yield return new WaitForSeconds(.2f);
                        Instantiate(baseattack, TravellerController.Instance.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 180f))));

                        yield return new WaitForSeconds(0.4f);
                    }
                    Debug.Log("0 End");
                    break;
                }
                else if (temp == 1)
                {
                    Debug.Log("1 Start");
                    ultStack++;
                    int ultCount = ultStack == 1 ? 0 : ultStack == 2 ? 2 : 5;

                    for (int i = 0; i < ultCount; i++)
                    {
                        Vector3 originpos = rigidbody2D.transform.position;
                        Vector3 targetpos = TravellerController.Instance.transform.position + new Vector3(((i + 2) % 2 == 1 ? 1 : -1) * Random.Range(5f, 10f), (-1 + Random.Range(0, 2) * 2) * Random.Range(5f, 10f));

                        for (float j = 0; j <= 1; j += 0.02f * 5)
                        {
                            rigidbody2D.transform.position = Vector3.Lerp(originpos, targetpos, j);
                            yield return new WaitForSeconds(0.02f);
                        }
                    }

                    yield return new WaitForSeconds(0.5f);

                    Vector3 aoriginpos = rigidbody2D.transform.position;
                    Vector3 atargetpos = TravellerController.Instance.transform.position;

                    for (float j = 0; j <= 1; j += 0.02f * 5)
                    {
                        rigidbody2D.transform.position = Vector3.Lerp(aoriginpos, atargetpos, j);
                        yield return new WaitForSeconds(0.02f);
                    }

                    yield return new WaitForSeconds(1f);
                    Debug.Log("1 End");
                }
            }
            else
            {
                t += Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                Debug.LogWarning("t");
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
