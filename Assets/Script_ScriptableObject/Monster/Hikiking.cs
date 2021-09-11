using System.Collections;
using UnityEngine;

public class Hikiking : BossMonster
{
    [SerializeField] private GameObject baseattack;
    int ultStack = 0;

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

    protected override void Update()
    {
        spriteRenderer.sortingOrder = -(int)System.Math.Truncate(transform.position.y * 10);

        UIManager.Instance.redParent.transform.localScale = new Vector3(Mathf.Lerp(UIManager.Instance.redParent.transform.localScale.x, (float)HP / maxHP, Time.deltaTime * 30f), 1, 1);
    }

    private IEnumerator Attack()
    {
        Debug.Log($"{name} : Now Attack");

        while (true)
        {
            int i = Random.Range(0, 2 + 1);
            switch (i)
            {
                case 0:
                    Debug.Log($"{name} : 0 Call BaseAttack");
                    yield return StartCoroutine(BaseAttack());
                    break;
                case 1:
                    Debug.Log($"{name} : 1 Call ULT");
                    yield return StartCoroutine(ULT());
                    break;
                case 2:
                    Debug.Log($"{name} : 2 Call Skill1");
                    yield return StartCoroutine(Skill1());
                    break;
                default:
                    Debug.LogError($"{name} : Á¸ÀçÇÏÁö ¾Ê´Â °ø°Ý ÀÎµ¦½º");
                    break;
            }

            Debug.Log($"{name} : Wait");
            yield return new WaitForSeconds(3f);
            Debug.Log($"{name} : WaitEnd");
        }      
    }

    /// <summary>
    /// ¿Î±» ÁÖº¯À¸·Î ÀÌµ¿ ÈÄ ½»½» ±Ü±â
    /// </summary>
    private IEnumerator BaseAttack()
    {
        Debug.Log($"{name} : 0 BaseAttack Start");
        int attackcount = Random.Range(2, 3 + 1);

        for (int i = 0; i < attackcount; i++)
        {
            Vector3 originpos = rigidbody2D.transform.position;
            Vector3 targetpos = TravellerController.Instance.transform.position + new Vector3((-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f), (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

            for (float j = 0; j <= 1; j += 0.02f * 10)
            {
                rigidbody2D.transform.position = Vector3.Lerp(originpos, targetpos, j);
                yield return new WaitForSeconds(0.02f);
            }

            yield return new WaitForSeconds(0.2f);

            int slashCount = Random.Range(2, 4 + 1);
            for (int k = 0; k < slashCount; k++)
            {
                Instantiate(baseattack, TravellerController.Instance.transform.position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 180f))));
                yield return new WaitForSeconds(.1f);
            }        

            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log($"{name} : 0 BaseAttack End");
    }

    /// <summary>
    /// ´À±×ÀÏ¼¶. 1 > 3 > 6 ¹ø
    /// </summary>
    private IEnumerator ULT()
    {
        Debug.Log($"{name} : 1 ULT Start");
        UIManager.Instance.aaa.text = "°ø±â°¡ ¿äµ¿Ä¨´Ï´Ù...";
        UIManager.Instance.aaa.gameObject.SetActive(true);

        // ¿¬Ãâ
        yield return new WaitForSeconds(1f);

        UIManager.Instance.aaa.gameObject.SetActive(false);

        ultStack++;
        int ultCount = ultStack == 1 ? 0 : ultStack == 2 ? 2 : 5;

        for (int i = 0; i < ultCount; i++)
        {
            Vector3 originpos = rigidbody2D.transform.position;
            Vector3 targetpos = TravellerController.Instance.transform.position + new Vector3(((i + 2) % 2 == 1 ? 1 : -1) * Random.Range(5f, 10f), (-1 + Random.Range(0, 2) * 2) * Random.Range(5f, 10f));

            for (float j = 0; j <= 1; j += 0.02f * 10)
            {
                rigidbody2D.transform.position = Vector3.Lerp(originpos, targetpos, j);
                yield return new WaitForSeconds(0.02f);
            }
            Instantiate(baseattack, originpos + (targetpos - originpos) / 2, Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(targetpos.y - originpos.y, targetpos.x - originpos.x)))).transform.localScale = new Vector3(Vector3.Distance(originpos, targetpos) / 3, 1, 1);
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 aoriginpos = transform.position;
        Vector3 atargetpos = TravellerController.Instance.transform.position + (TravellerController.Instance.transform.position - transform.position).normalized * 5;

        for (float j = 0; j <= 1; j += 0.02f * 10)
        {
            rigidbody2D.transform.position = Vector3.Lerp(aoriginpos, atargetpos, j);
            yield return new WaitForSeconds(0.02f);
        }
        Instantiate(baseattack, aoriginpos + (atargetpos - aoriginpos) / 2, Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(atargetpos.y - aoriginpos.y, atargetpos.x - aoriginpos.x)))).transform.localScale = new Vector3(Vector3.Distance(aoriginpos, atargetpos) / 3, 3, 1);

        yield return new WaitForSeconds(1f);
        Debug.Log($"{name} : 1 ULT End");
    }

    /// <summary>
    /// ¸Ê¿¡ Â¬»ì´ë »Ñ¸®±â
    /// </summary>
    private IEnumerator Skill1()
    {
        int jjabSlayerCount = Random.Range(1, 5 + 1);
        for (int i = 0; i < jjabSlayerCount; i++)
        {
            yield return new WaitForSeconds(0.2f);
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
