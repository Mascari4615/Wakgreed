using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pungsin : BossMonster
{
    [SerializeField] private GameObject ultAttackGo;
    [SerializeField] private GameObject ult;
    [SerializeField] private GameObject ultParticle1;
    [SerializeField] private GameObject ultParticle2;
    private GameObject[] ultAttackPos;
    private BulletMove[] ultAttackGos;
    [SerializeField] private GameObject stun;

    protected override void Awake()
    {
        base.Awake();
        // skill1 = transform.Find("Skill1").gameObject;
        ult = transform.Find("ult").gameObject;
        ultAttackPos = new GameObject[ult.transform.childCount];
        ultAttackGos = new BulletMove[ult.transform.childCount];
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
        }
    }

    private IEnumerator Ult()
    {
        Animator.SetBool("ULT", true);
        cinemachineTargetGroup.m_Targets[1].target = transform;

        float temp = 0;
        while (temp < .2f)
        {
            if (camera.m_Lens.OrthographicSize < 17) camera.m_Lens.OrthographicSize += 8 * Time.fixedDeltaTime;
            Wakgood.Instance.WakgoodMove.PlayerRb.AddForce((transform.position - Wakgood.Instance.transform.position).normalized * 600);
            yield return null;
            temp += Time.deltaTime;
        }

        ult.SetActive(true);
        ultParticle1.SetActive(true);

        temp = 0;
        while (temp < 4f)
        {
            if (camera.m_Lens.OrthographicSize > 12) camera.m_Lens.OrthographicSize -= 6 * Time.fixedDeltaTime;
            else if (camera.m_Lens.OrthographicSize > 10) camera.m_Lens.OrthographicSize -= 1 * Time.fixedDeltaTime;
            Wakgood.Instance.WakgoodMove.PlayerRb.AddForce((transform.position - Wakgood.Instance.transform.position).normalized * 600);
            yield return new WaitForFixedUpdate();
            temp += Time.fixedDeltaTime;
        }
        ultParticle1.SetActive(false);

        ultParticle2.SetActive(true);
        for (int i = 0; i < ultAttackGos.Length; i++)
        {
            ultAttackGos[i].transform.position = transform.position;
            ultAttackGos[i].SetDirection((ultAttackPos[i].transform.position - transform.position).normalized);
            ultAttackGos[i].gameObject.SetActive(true);
        }

        while (camera.m_Lens.OrthographicSize < 12)
        {
            camera.m_Lens.OrthographicSize += 5 * Time.deltaTime;
            yield return null;
        }

        ult.SetActive(false);
        Animator.SetBool("ULT", false);
        camera.m_Lens.OrthographicSize = 12;

        stun.SetActive(true);
        yield return new WaitForSeconds(6f);
        stun.SetActive(false);
        ultParticle2.SetActive(false);
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