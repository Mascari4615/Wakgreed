using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTeam : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
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
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        Vector2 spawnPos = transform.position;
        float t = 0;
        Transform target = GetNearestMob();

        while (target == null)
        {
            yield return new WaitForSeconds(0.3f);
            target = GetNearestMob();
        }

        while (true)
        {
            yield return null;
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(spawnPos, target.position, t);
            spriteRenderer.flipX = (target.position.x > transform.position.x) ? true : false;
        }
    }
}
