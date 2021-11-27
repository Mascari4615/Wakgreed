using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Hikiking : BossMonster
{
    [SerializeField] private GameObject baseAttack;
    [SerializeField] private TextMeshProUGUI text;
    private int ultStack = 0;

    protected override IEnumerator Attack()
    {
        while (true)
        {
            int i = Random.Range(0, 1 + 1);
            switch (i)
            {
                case 0:
                    yield return StartCoroutine(BaseAttack());
                    break;
                case 1:
                    yield return StartCoroutine(Ult());
                    break;
                case 2:
                    yield return StartCoroutine(Skill1());
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator BaseAttack()
    {
        int attackCount = Random.Range(2, 3 + 1);

        for (int i = 0; i < attackCount; i++)
        {
            Vector3 originPos = Rigidbody2D.transform.position;
            Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3(
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f),
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

            for (float j = 0; j <= 1; j += 0.02f * 10)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return new WaitForSeconds(0.02f);
            }

            yield return new WaitForSeconds(0.2f);

            int slashCount = Random.Range(2, 4 + 1);
            for (int k = 0; k < slashCount; k++)
            {
                Instantiate(baseAttack, Wakgood.Instance.transform.position,
                    Quaternion.Euler(new Vector3(0, 0, Random.Range(0f, 180f))));
                yield return new WaitForSeconds(.1f);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator Ult()
    {
        text.text = "공기가 요동칩니다...";
        text.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        text.gameObject.SetActive(false);

        ultStack++;
        int ultCount = ultStack switch
        {
            1 => 0,
            2 => 2,
            _ => 5
        };

        for (int i = 0; i < ultCount; i++)
        {
            Vector3 originPos = Rigidbody2D.transform.position;
            Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3(
                ((i + 2) % 2 == 1 ? 1 : -1) * Random.Range(5f, 10f),
                (-1 + Random.Range(0, 2) * 2) * Random.Range(5f, 10f));

            for (float j = 0; j <= 1; j += 0.02f * 15)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return new WaitForSeconds(0.02f);
            }

            Instantiate(baseAttack, originPos + (targetPos - originPos) / 2,
                    Quaternion.Euler(new Vector3(0, 0,
                        Mathf.Rad2Deg * Mathf.Atan2(targetPos.y - originPos.y, targetPos.x - originPos.x)))).transform
                .localScale = new Vector3(Vector3.Distance(originPos, targetPos) / 3, 1, 1);
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 aOriginPos = transform.position;
        Vector3 aTargetPos = Wakgood.Instance.transform.position +
                             (Wakgood.Instance.transform.position - transform.position).normalized * 5;

        for (float j = 0; j <= 1; j += 0.02f * 20)
        {
            Rigidbody2D.transform.position = Vector3.Lerp(aOriginPos, aTargetPos, j);
            yield return new WaitForSeconds(0.02f);
        }

        Instantiate(baseAttack, aOriginPos + (aTargetPos - aOriginPos) / 2,
                Quaternion.Euler(new Vector3(0, 0,
                    Mathf.Rad2Deg * Mathf.Atan2(aTargetPos.y - aOriginPos.y, aTargetPos.x - aOriginPos.x)))).transform
            .localScale = new Vector3(Vector3.Distance(aOriginPos, aTargetPos) / 3, 3, 1);
    }

    private IEnumerator Skill1()
    {
        int jjabSlayerCount = Random.Range(1, 5 + 1);
        for (int i = 0; i < jjabSlayerCount; i++)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }
}