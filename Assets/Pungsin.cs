using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pungsin : BossMonster
{
    [SerializeField] private GameObject ultAttackGo;
    [SerializeField] private GameObject ult;
    private GameObject[] ultAttackPos;
    private BulletMove[] ultAttackGos;

    protected override void Awake()
    {
        base.Awake();
        // skill1 = transform.Find("Skill1").gameObject;
        ult = transform.Find("ult").gameObject;
        ultAttackPos = new GameObject[ult.transform.childCount];
        for (int i = 0; i < ult.transform.childCount; i++)
        {
            ultAttackPos[i] = ult.transform.GetChild(i).gameObject;
            (ultAttackGos[i] = Instantiate(ultAttackGo, transform).GetComponent<BulletMove>()).gameObject.SetActive(false);
        }
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            int i = Random.Range(0, 1 + 1);
            switch (i)
            {
                case 0:
                    yield return StartCoroutine(Skill0());
                    break;
                case 1:
                    yield return StartCoroutine(Ult());
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Ult()
    {
        int temp = 0;

        ult.SetActive(true);

        while (temp < 40)
        {
            Wakgood.Instance.WakgoodMove.PlayerRb.AddForce((transform.position - Wakgood.Instance.transform.position).normalized * 200);
            yield return new WaitForSeconds(.1f);
            temp++;
        }

        for (int i = 0; i < ultAttackGos.Length; i++)
        {
            ultAttackGos[i].SetDirection((ultAttackPos[i].transform.position - transform.position).normalized);
            ultAttackGos[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(.2f);
        }

        ult.SetActive(false);
    }

    private IEnumerator Skill0()
    {
        yield return new WaitForSeconds(.2f);

        /*
        if (t == 0)
        {
            spawnPosParent.gameObject.SetActive(true);
            rand1 = Random.Range(-40, 41);
            spawnPosParent.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y - GameManager.Instance.player.transform.position.y, transform.position.x - GameManager.Instance.player.transform.position.x) * Mathf.Rad2Deg - 90 + rand1);
        }
        t += Time.deltaTime;
        if (t < 1.5f)
        {
            spawnPosParent.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(Mathf.Atan2(transform.position.y - GameManager.Instance.player.transform.position.y, transform.position.x - GameManager.Instance.player.transform.position.x) * Mathf.Rad2Deg - 90 + rand1, Mathf.Atan2(transform.position.y - GameManager.Instance.player.transform.position.y, transform.position.x - GameManager.Instance.player.transform.position.x) * Mathf.Rad2Deg - 90, t * 0.3f));
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
        */
    }
}