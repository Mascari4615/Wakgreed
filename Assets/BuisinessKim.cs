using System.Collections;
using FMODUnity;
using UnityEngine;

public class BuisinessKim : BossMonster
{
    [SerializeField] private GameObject skill0AttackPrefab;
    private BSlash[] skill0AttackGo = new BSlash[2];
    // [SerializeField] private float moveLimit = 15;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject panel;
    private Vector3 spawnedPos = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();

        (skill0AttackGo[0] = Instantiate(skill0AttackPrefab, transform).GetComponent<BSlash>()).gameObject.SetActive(false);
        (skill0AttackGo[1] = Instantiate(skill0AttackPrefab, transform).GetComponent<BSlash>()).gameObject.SetActive(false);
        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        spawnedPos = transform.position;
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            int i = Random.Range(0, 2 + 1);
            switch (i)
            {
                case 0:
                    yield return StartCoroutine(Skill0());
                    break;
                case 1:
                    yield return StartCoroutine(ULT());
                    break;
                case 2:
                    yield return StartCoroutine(ULT());
                    break;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    protected IEnumerator Skill0()
    {
        yield return new WaitForSeconds(.2f);
        Animator.SetBool("SKILL1", true);

        Vector3 originPos = transform.position;
        Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3(
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f),
                (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

        Animator.SetTrigger("SKILL1CHARGE");

        SpriteRenderer.flipX = targetPos.x > Wakgood.Instance.transform.position.x;

        for (float j = 0; j <= 1; j += Time.deltaTime * 7)
        {
            Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
            yield return null;
        }

        Vector3 attackDirection = (Wakgood.Instance.transform.position - transform.position).normalized;

        lineRenderer.SetPosition(0, transform.position + (Vector3)Vector2.up);
        lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + attackDirection * 100);
        lineRenderer.gameObject.SetActive(true);
        float angle = Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 1), Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        yield return new WaitForSeconds(.4f);

        Animator.SetTrigger("SKILL1GO");

        skill0AttackGo[0].transform.position = transform.position + (Vector3)Vector2.up;
        skill0AttackGo[0].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        skill0AttackGo[0].SetDirection(attackDirection);
        skill0AttackGo[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(.4f);
        skill0AttackGo[1].transform.position = transform.position + (Vector3)Vector2.up;
        skill0AttackGo[1].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        skill0AttackGo[1].SetDirection(attackDirection);
        skill0AttackGo[1].gameObject.SetActive(true);
        lineRenderer.gameObject.SetActive(false);

        Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(2f);
    }

    protected IEnumerator ULTSkill0()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(.2f);
            Animator.SetBool("SKILL1", true);

            Vector3 originPos = transform.position;
            Vector3 targetPos = Wakgood.Instance.transform.position + new Vector3(
                    (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f),
                    (-1 + Random.Range(0, 2) * 2) * Random.Range(3f, 5f));

            Animator.SetTrigger("SKILL1CHARGE");

            SpriteRenderer.flipX = targetPos.x > Wakgood.Instance.transform.position.x;

            for (float j = 0; j <= 1; j += Time.deltaTime * 7)
            {
                Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
                yield return null;
            }

            Vector3 attackDirection = (Wakgood.Instance.transform.position - transform.position).normalized;

            lineRenderer.SetPosition(0, transform.position + (Vector3)Vector2.up);
            lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + attackDirection * 100);
            lineRenderer.gameObject.SetActive(true);
            float angle = Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 1), Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            yield return new WaitForSeconds(.6f);

            Animator.SetTrigger("SKILL1GO");

            skill0AttackGo[0].transform.position = transform.position + (Vector3)Vector2.up;
            skill0AttackGo[0].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            skill0AttackGo[0].SetDirection(attackDirection);
            skill0AttackGo[0].gameObject.SetActive(true);
            yield return new WaitForSeconds(.2f);
            skill0AttackGo[1].transform.position = transform.position + (Vector3)Vector2.up;
            skill0AttackGo[1].transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            skill0AttackGo[1].SetDirection(attackDirection);
            skill0AttackGo[1].gameObject.SetActive(true);
            lineRenderer.gameObject.SetActive(false);

            Animator.SetBool("SKILL1", false);
            yield return new WaitForSeconds(.5f);
        }

    }

    protected IEnumerator ULT()
    {
        //
        StartCoroutine(ULT_Grab());
        panel.SetActive(true);

        int ultCount = Random.Range(6, 10 + 1);
        yield return new WaitForSeconds(.3f);

        for (int i = 0; i < ultCount; i++)
        {
            lineRenderer.gameObject.SetActive(true);
            Vector3 targetPos = Wakgood.Instance.transform.position + Vector3.up * Random.Range(-5f, 5f);
            Vector3 originPos = targetPos + Vector3.up * 100;
            Vector3 originPos2 = targetPos + Vector3.down * 100;
            lineRenderer.SetPosition(0, originPos);
            lineRenderer.SetPosition(1, originPos2);
            yield return new WaitForSeconds(.6f);
            lineRenderer.gameObject.SetActive(false);

            var temp = ObjectManager.Instance.PopObject("BuisSlash", originPos + (originPos2 - originPos) / 2,
             new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(originPos2.y - originPos.y, originPos2.x - originPos.x)));
            temp.transform.localScale = new Vector3(Vector3.Distance(originPos, originPos2) * 0.25f, 1, 1);
            RuntimeManager.PlayOneShot($"event:/SFX/Weapon/2");

            yield return new WaitForSeconds(0.2f);
        }

        lineRenderer.gameObject.SetActive(false);

        yield return new WaitForSeconds(3f);
        panel.SetActive(false);
    }

    protected IEnumerator ULT_Grab()
    {
        float temp = 0;
        while (temp < 7f)
        {
/*            if (camera.m_Lens.OrthographicSize > 12) camera.m_Lens.OrthographicSize -= 6 * Time.fixedDeltaTime;
            else if (camera.m_Lens.OrthographicSize > 10) camera.m_Lens.OrthographicSize -= 1 * Time.fixedDeltaTime;*/
            // Wakgood.Instance.WakgoodMove.PlayerRb.AddForce(Vector3.left * 500);
            yield return new WaitForFixedUpdate();
            temp += Time.fixedDeltaTime;
        }
    }
}
