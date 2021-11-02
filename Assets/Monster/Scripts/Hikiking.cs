using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Hikiking : BossMonster
{
    [FormerlySerializedAs("baseattack")] [SerializeField] private GameObject baseAttack;
    private int ultStack = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Ready());
    }

    private IEnumerator Ready()
    {
        UIManager.Instance.bossHpBar.SetActive(true);
        yield return StartCoroutine(UIManager.Instance.SpeedWagon_Boss(gameObject));
        yield return new WaitForSeconds(2f);
        StartCoroutine(Attack());
    }

    private void Update()
    {
        UIManager.Instance.redParent.transform.localScale = new Vector3(Mathf.Lerp(UIManager.Instance.redParent.transform.localScale.x, (float)Hp / MAXHp, Time.deltaTime * 30f), 1, 1);
    }

    private IEnumerator Attack()
    {
        //Debug.Log($"{name} : Now Attack");

        while (true)
        {
            int i = Random.Range(0, 1 + 1);
            switch (i)
            {
                case 0:
                    //Debug.Log($"{name} : 0 Call BaseAttack");
                    yield return StartCoroutine(BaseAttack());
                    break;
                case 1:
                    //Debug.Log($"{name} : 1 Call ULT");
                    yield return StartCoroutine(Ult());
                    break;
                case 2:
                    //Debug.Log($"{name} : 2 Call Skill1");
                    yield return StartCoroutine(Skill1());
                    break;
                default:
                    //Debug.LogError($"{name} : �������� �ʴ� ���� �ε���");
                    break;
            }

            //Debug.Log($"{name} : Wait");
            yield return new WaitForSeconds(2f);
            //Debug.Log($"{name} : WaitEnd");
        }      
    }

    private IEnumerator BaseAttack()
    {
        //Debug.Log($"{name} : 0 BaseAttack Start");
        int attackCount = Random.Range(2, 3 + 1);

        for (int i = 0; i < attackCount; i++)
        {
            Vector3 originPos = Rigidbody2D.transform.position;
            Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3((-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f), (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

            for (float j = 0; j <= 1; j += 0.02f * 10)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return new WaitForSeconds(0.02f);
            }

            yield return new WaitForSeconds(0.2f);

            int slashCount = Random.Range(2, 4 + 1);
            for (int k = 0; k < slashCount; k++)
            {
                Instantiate(baseAttack, Wakgood.Instance.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 180f))));
                yield return new WaitForSeconds(.1f);
            }        

            yield return new WaitForSeconds(0.2f);
        }
        //Debug.Log($"{name} : 0 BaseAttack End");
    }

    private IEnumerator Ult()
    {
        UIManager.Instance.aaa.text = "공기가 요동칩니다...";
        UIManager.Instance.aaa.gameObject.SetActive(true);

        // ����
        yield return new WaitForSeconds(1f);

        UIManager.Instance.aaa.gameObject.SetActive(false);

        ultStack++;
        int ultCount = ultStack == 1 ? 0 : ultStack == 2 ? 2 : 5;

        for (int i = 0; i < ultCount; i++)
        {
            Vector3 originPos = Rigidbody2D.transform.position;
            Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3(((i + 2) % 2 == 1 ? 1 : -1) * Random.Range(5f, 10f), (-1 + Random.Range(0, 2) * 2) * Random.Range(5f, 10f));

            for (float j = 0; j <= 1; j += 0.02f * 10)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return new WaitForSeconds(0.02f);
            }
            Instantiate(baseAttack, originPos + (targetPos - originPos) / 2, Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(targetPos.y - originPos.y, targetPos.x - originPos.x)))).transform.localScale = new Vector3(Vector3.Distance(originPos, targetPos) / 3, 1, 1);
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 aOriginPos = transform.position;
        Vector3 aTargetPos = Wakgood.Instance.transform.position + (Wakgood.Instance.transform.position - transform.position).normalized * 5;

        for (float j = 0; j <= 1; j += 0.02f * 10)
        {
            Rigidbody2D.transform.position = Vector3.Lerp(aOriginPos, aTargetPos, j);
            yield return new WaitForSeconds(0.02f);
        }
        Instantiate(baseAttack, aOriginPos + (aTargetPos - aOriginPos) / 2, Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(aTargetPos.y - aOriginPos.y, aTargetPos.x - aOriginPos.x)))).transform.localScale = new Vector3(Vector3.Distance(aOriginPos, aTargetPos) / 3, 3, 1);

        //Debug.Log($"{name} : 1 ULT End");
    }

    private IEnumerator Skill1()
    {
        int jjabSlayerCount = Random.Range(1, 5 + 1);
        for (int i = 0; i < jjabSlayerCount; i++)
        {
            yield return new WaitForSeconds(0.2f);
        }
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
