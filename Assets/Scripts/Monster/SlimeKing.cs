using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeKing : Monster
{
    private bool isReadyToAttack = false;
    private float t = 0;
    private float attackCoolTime = 5;
    [SerializeField] private Transform spawnPosParent = null;
    [SerializeField] private Transform spawnPos = null;

    [SerializeField] private GameObject attack222 = null;
    [SerializeField] private GameObject asdasdasd = null;

    [SerializeField] private GameObject attack333333 = null;
    private float force = 2000;
    int rand = 0;
    int rand1 = 0;

    void Awake()
    {
        isBoss = true;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        base.Update();
        Attack();
    }

    protected override void Attack()
    {
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
                        spawnPosParent.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - Traveller.Instance.transform.position.y,  transform.position.x - Traveller.Instance.transform.position.x) * Mathf.Rad2Deg - 90 + rand1);
                    }
                    t += Time.deltaTime;
                    if (t < 1.5f)
                    {          
                        spawnPosParent.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(Mathf.Atan2(transform.position.y - Traveller.Instance.transform.position.y,  transform.position.x - Traveller.Instance.transform.position.x) * Mathf.Rad2Deg - 90 + rand1, Mathf.Atan2(transform.position.y - Traveller.Instance.transform.position.y,  transform.position.x - Traveller.Instance.transform.position.x) * Mathf.Rad2Deg - 90, t * 0.3f));
                    }
                    else if (t >= 1.5f)
                    {
                        isReadyToAttack = false;
                        t = 0;

                        GameObject g = ObjectManager.Instance.GetQueue(PoolType.Slime2, spawnPos.position);
                        GameManager.Instance.monsters.Add(g);
                        
                        g.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        g.GetComponent<Rigidbody2D>().AddForce((Vector2)(spawnPos.position - transform.position).normalized * force);

                        spawnPosParent.gameObject.SetActive(false);
                    }
                break;

                case 1 : // 몸통 박치기
                    if (t == 0)
                    {
                        attack222.SetActive(true);
                        attack222.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - Traveller.Instance.transform.position.y,  transform.position.x - Traveller.Instance.transform.position.x) * Mathf.Rad2Deg - 90 );
                    }
                    t += Time.deltaTime;
                    if (t < 3 && t >= 1)
                    {
                        monsterRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                        monsterRigidbody2D.velocity = (Vector2)(asdasdasd.transform.position - transform.position).normalized * moveSpeed;
                        attack222.gameObject.SetActive(false);
                    }
                    else if (t >= 3)
                    {  
                        monsterRigidbody2D.velocity = Vector2.zero;
                        monsterRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
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
                        for (int i = 0; i < ObjectManager.Instance.monsterParent.childCount; i++)
                        {
                            ObjectManager.Instance.monsterParent.GetChild(i).GetComponent<Monster>().monsterRigidbody2D.AddForce((transform.position - ObjectManager.Instance.monsterParent.GetChild(i).transform.position).normalized * 200);
                        }
                        Traveller.Instance.GetComponent<Rigidbody2D>().AddForce((transform.position - Traveller.Instance.transform.position).normalized * 200);
                    }
                    if (t > 9)
                    {
                        attack333333.SetActive(false);
                        isReadyToAttack = false;
                        t = 0;
                    }
                break;

                default :
                    Debug.Log("SlimeKing ERROR");
                break;
            } 
        }
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);
        int rand = Random.Range(0, 100);
        if (rand <= 5)
        {
            GameManager.Instance.monsters.Add(ObjectManager.Instance.GetQueue(PoolType.Slime1, transform.position));
        }
    }

    protected override void InsertQueue()
    {
        base.InsertQueue();
        ObjectManager.Instance.InsertQueue(PoolType.BossMonster, gameObject);
    }
}
