using System.Collections;
using UnityEngine;

public class GhostTeam : MonoBehaviour
{
    private SpriteRenderer SpriteRenderer;
    private readonly WaitForSeconds ws01 = new(0.1f);

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(Move());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Move()
    {
        while (true)
        {
            Vector2 spawnPos = transform.position;
            Transform target = GameManager.Instance.GetNearestMob(transform);

            if (target != null)
            {
                for (float j = 0; j <= 1; j += Time.deltaTime)
                {
                    if (target.gameObject.activeSelf == false || target == null)
                    {
                        break;
                    }

                    SpriteRenderer.flipX = target.position.x > transform.position.x ? true : false;
                    transform.position = Vector3.Lerp(spawnPos, target.position, j);
                    yield return null;
                }
            }

            yield return ws01;
        }
    }
}