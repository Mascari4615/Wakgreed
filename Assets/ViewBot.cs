using System.Collections;
using UnityEngine;
using FMODUnity;
using System;

public class ViewBot : NormalMonster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        SpriteRenderer.color = Color.white;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            Animator.SetBool("ISMOVING", true);
            Rigidbody2D.velocity = (Wakgood.Instance.transform.position - transform.position).normalized * MoveSpeed;
            yield return new WaitForSeconds(0.1f);

            if (Vector3.Distance(Wakgood.Instance.transform.position, transform.position) < 1f)
            {
                StartCoroutine(Boom());
                yield break;
            }
        }
    }

    private IEnumerator Boom()
    {
        for (int i = 0; i < 4; i++)
        {
            Animator.SetBool("ISMOVING", false);
            SpriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            SpriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        SpriteRenderer.material = originalMaterial;

        ObjectManager.Instance.PopObject("ViewBotBoom", transform.position);
        isCollapsed = true;
        RuntimeManager.PlayOneShot($"event:/SFX/Monster/{(name.Contains("(Clone)") ? name.Remove(name.IndexOf("(", StringComparison.Ordinal), 7) : name)}_Collapse", transform.position);

        if (StageManager.Instance.CurrentRoom is NormalRoom)
        {
            onMonsterCollapse.Raise(transform);
        }

        collider2D.enabled = false;
        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.bodyType = RigidbodyType2D.Static;
        GameManager.Instance.enemyRunTimeSet.Remove(gameObject);

        gameObject.SetActive(false);
    }
}
