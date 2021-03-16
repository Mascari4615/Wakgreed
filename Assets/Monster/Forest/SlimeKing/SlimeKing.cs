using UnityEngine;
using System.Collections;

public class SlimeKing : Monster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
        }
/*
        if (isReadyToAttack == false)
        {
            t += Time.deltaTime;

            if (t >= attackCoolTime)
            {
                t = 0;
                isReadyToAttack = true; 
                rand = Random.Range(0, 3);
                Debug.Log(rand);
            }
        }
        else
        {
            switch (rand)
            {
                case 0 : // 슬라임 발사
                    if (t == 0)
                    {
                        spawnPosParent.gameObject.SetActive(true);
                        rand1 = Random.Range(-40, 41);
                        spawnPosParent.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - TravellerController.Instance.transform.position.y,  transform.position.x - TravellerController.Instance.transform.position.x) * Mathf.Rad2Deg - 90 + rand1);
                    }
                    t += Time.deltaTime;
                    if (t < 1.5f)
                    {          
                        spawnPosParent.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(Mathf.Atan2(transform.position.y - TravellerController.Instance.transform.position.y,  transform.position.x - TravellerController.Instance.transform.position.x) * Mathf.Rad2Deg - 90 + rand1, Mathf.Atan2(transform.position.y - TravellerController.Instance.transform.position.y,  transform.position.x - TravellerController.Instance.transform.position.x) * Mathf.Rad2Deg - 90, t * 0.3f));
                    }
                    else if (t >= 1.5f)
                    {
                        isReadyToAttack = false;
                        t = 0;

                        GameObject g = ObjectManager.Instance.GetQueue(PoolType.Slime2, spawnPos.position);
                        EnemyRunTimeSet.Add(g);
                        
                        g.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        g.GetComponent<Rigidbody2D>().AddForce((Vector2)(spawnPos.position - transform.position).normalized * force);

                        spawnPosParent.gameObject.SetActive(false);
                    }
                break;

                case 1 : // 몸통 박치기
                    if (t == 0)
                    {
                        attack222.SetActive(true);
                        attack222.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - TravellerController.Instance.transform.position.y,  transform.position.x - TravellerController.Instance.transform.position.x) * Mathf.Rad2Deg - 90 );
                    }
                    t += Time.deltaTime;
                    if (t < 3 && t >= 1)
                    {
                        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                        rigidbody2D.velocity = (Vector2)(asdasdasd.transform.position - transform.position).normalized * moveSpeed;
                        attack222.gameObject.SetActive(false);
                    }
                    else if (t >= 3)
                    {  
                        rigidbody2D.velocity = Vector2.zero;
                        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                        isReadyToAttack = false;
                        t = 0;
                    }
                break;

                case 2 : // 흡수
                    
                    if (t == 0)
                    {
                        attack333333.SetActive(true);
                    }
                    t += Time.deltaTime;
                    if (t > 2)
                    {
                        for (int i = 0; i < EnemyRunTimeSet.Items.Count; i++)
                        {
                            if (EnemyRunTimeSet.Items[0] == this) continue;
                            EnemyRunTimeSet.Items[i].GetComponent<Rigidbody2D>().AddForce((transform.position - EnemyRunTimeSet.Items[i].transform.position).normalized * 200);
                        }
                        TravellerController.Instance.GetComponent<Rigidbody2D>().AddForce((transform.position - TravellerController.Instance.transform.position).normalized * 200);
                    }
                    if (t > 9)
                    {
                        attack333333.SetActive(false);
                        isReadyToAttack = false;
                        t = 0;
                    }
                break;
            }
        }
        */
    }

    public override void ReceiveDamage(int damage, TextType damageType = TextType.Normal)
    {
        base.ReceiveDamage(damage, damageType);
        int rand = Random.Range(0, 100);
        if (rand <= 5)
        {
            EnemyRunTimeSet.Add(ObjectManager.Instance.GetQueue(PoolType.Slime1, transform.position));
        }
    }

    protected override void OnCollapse()
    {
        //throw new System.NotImplementedException();
    }
}
