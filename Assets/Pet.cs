using System.Collections;
using FMODUnity;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;
    private bool canWakggiddi = true;
    private float coolTime = 3f;

    void Start()
    {
        StartCoroutine(FollowWakgood());
    }

    private IEnumerator ChangeWithDelay(bool changeValue, float delay, System.Action<bool> makeResult)
    {
        // 참고 : https://velog.io/@sonohoshi/10.-Unity에서-일정-시간-이후-값을-바꾸는-방법
        yield return new WaitForSeconds(delay);
        makeResult(changeValue);
    }

    private IEnumerator FollowWakgood()
    {
        WaitForSeconds ws01 = new(0.1f);
        while (true)
        {
            if (canWakggiddi)
            {
                if (GameManager.Instance.EnemyRunTimeSet.Items.Count > 0)
                {
                    Vector3 originalDistance = transform.position - Wakgood.Instance.transform.position;
                    Transform mob = GetNearestMob();
                    float temp = 0;

                    while (Vector3.Distance(transform.position, mob.position) > 0.5f)
                    {
                        temp += Time.deltaTime;
                        transform.SetPositionAndRotation(Vector3.Lerp(transform.position, mob.position, temp * 1.5f), Quaternion.identity);
                        yield return null;
                    }
                    RuntimeManager.PlayOneShot($"event:/SFX/Item/Wakgi", transform.position);
                    mob.GetComponent<IHitable>().ReceiveHit(1);

                    if (3 >= Random.Range(1, 10 + 1))
                        Wakgood.Instance.ReceiveHeal(1);

                    temp = 0;
                    while (Vector3.Distance(transform.position, Wakgood.Instance.transform.position + originalDistance) > .5f)
                    {
                        temp += Time.deltaTime;
                        transform.position = Vector3.Lerp(transform.position, Wakgood.Instance.transform.position + originalDistance, temp * 1.5f);
                        yield return null;
                    }

                    canWakggiddi = false;
                    StartCoroutine(ChangeWithDelay(true, coolTime, (value) => canWakggiddi = value));
                }
            }

            transform.position = Vector3.Lerp(transform.position, Wakgood.Instance.transform.position + (transform.position - Wakgood.Instance.transform.position).normalized * 2, Time.deltaTime * moveSpeed);

            yield return null;
        }
    }

    private Transform GetNearestMob()
    {
        Transform target = null;
        float targetDist = 100;
        float currentDist;

        foreach (GameObject monster in GameManager.Instance.EnemyRunTimeSet.Items)
        {
            currentDist = Vector2.Distance(transform.position, monster.transform.position);
            if (currentDist > targetDist) continue;

            target = monster.transform;
            targetDist = currentDist;
        }

        return target;
    }
}
