using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime2 : Monster
{
    private bool isReadyToAttack = false;
    private float t = 0;
    private float attackCoolTime = 0;
    private bool isTargeting = false;
    [SerializeField] private GameObject slime = null;
    protected override void OnEnable()
    {
        int i = GameManager.Instance.currentStageNumber;
        base.OnEnable();

        isReadyToAttack = false;
        isTargeting = false;
        t = 0;
        attackCoolTime = 1;
        slime.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        Attack();
        Targeting();
    }

    void Attack()
    {
        if (isReadyToAttack == false)
        {
            t += Time.deltaTime;

            if (t >= attackCoolTime)
            {
                t = 0;
                attackCoolTime = 3;
                isReadyToAttack = true;
            }
        }
        else if (isTargeting)
        {
            if (t == 0)
            {
                monsterAnimator.SetTrigger("IsAttack");
            }

            t += Time.deltaTime;
            if (t >= 0.7f)
            {
                t = 0;
                slime.transform.localPosition = new Vector3(0, 0, -5);
                slime.GetComponent<ProjectiveObject>().SetDirection();
                slime.SetActive(true);
                isReadyToAttack = false;
            }
        }
    }

    private void Targeting()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, GameManager.Instance.player.transform.position - transform.position, Vector2.Distance(transform.position, GameManager.Instance.player.transform.position), LayerMask.NameToLayer("Everything"));
        for (int j = 0; j < hit.Length; j++)
        {
            if (hit[j].transform.CompareTag("Wall"))
            {
                isTargeting = false;
                break;
            }
            else if (hit[j].transform.CompareTag("Player"))
            {                     
                isTargeting = true;
            }
        }
 
        if (isTargeting)
        {
            Debug.DrawRay(transform.position, GameManager.Instance.player.transform.position - transform.position, Color.red);

            if (GameManager.Instance.player.transform.position.x > transform.position.x)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
            
            //attackPosParent.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(joyStick.inputValue.y, joyStick.inputValue.x) * Mathf.Rad2Deg - 90);
            //attackPos.transform.localPosition = new Vector3(0, attackPosGap, 0);
        }
    }

    public override void InsertQueue()
    {
        ObjectManager.Instance.InsertQueue(PoolType.Slime2, gameObject);
    }
}
