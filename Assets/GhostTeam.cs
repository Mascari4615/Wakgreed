using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTeam : MonoBehaviour
{
    private SpriteRenderer SpriteRenderer;
    private Coroutine move;
    private Coroutine idle;

    private void Awake() => SpriteRenderer = GetComponent<SpriteRenderer>();

    private Transform GetNearestMob()
    {
        Transform target = null;
        float targetDist = 100;
        float currentDist;

        foreach (GameObject monster in GameManager.Instance.enemyRunTimeSet.Items)
        {
            currentDist = Vector2.Distance(transform.position, monster.transform.position);
            if (currentDist > targetDist) continue;

            target = monster.transform;
            targetDist = currentDist;
        }

        return target;
    }

    private void OnEnable()
    {
        idle = StartCoroutine(Idle());
        move = StartCoroutine(Move());
    }

    private IEnumerator Idle()
    {
        while (true)
        {
            float t = Random.Range(1f, 3f);
            Vector2 direction = (Vector3)Random.insideUnitCircle.normalized;
;
            SpriteRenderer.flipX = direction.x < 0;

            while (t > 0)
            {
                yield return null;

                transform.position += (Vector3)direction * Time.deltaTime;
                t -= Time.deltaTime;
            }

            yield return new WaitForSeconds(.5f);
        }
    }

    private IEnumerator Move()
    {
        Vector2 spawnPos = transform.position;
        Transform target = GetNearestMob();

        while (target == null)
        {
            yield return new WaitForSeconds(0.3f);
            target = GetNearestMob();
        }

        StopCoroutine(idle);

        for (float j = 0; j <= 1; j += 0.02f)
        {
            if (target.gameObject.activeSelf == false || target == null)
            {
                Temp();
                yield break;
            }

            SpriteRenderer.flipX = (target.position.x > transform.position.x) ? true : false;
            transform.position = Vector3.Lerp(spawnPos, target.position, j);
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void Temp()
    {
        if (move != null) StopCoroutine(move);
        idle = StartCoroutine(Idle());
        move = StartCoroutine(Move());
    }

    private void OnDisable()
    {
        if (move != null) StopCoroutine(move);
        if (idle != null) StopCoroutine(idle);
    }
}
