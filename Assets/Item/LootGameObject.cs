using FMODUnity;
using System.Collections;
using UnityEngine;

public abstract class LootGameObject : MonoBehaviour
{
    private CircleCollider2D circleCollider2D;
    protected SpriteRenderer spriteRenderer;
   
    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        circleCollider2D.enabled = false;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        const float speed = 0.03f;
        Vector3 waitPosition = transform.position + new Vector3(Random.Range(-1.5f, 1.6f), Random.Range(-1.5f, 1.6f), 0);
        float temp = 0;
        
        while (temp < 1)
        {
            temp += speed;
            transform.position = Vector3.Lerp(transform.position, waitPosition, temp);
            yield return new WaitForSeconds(0.02f);
        }

        circleCollider2D.enabled = true;

        temp = 0;
        while (true)
        {
            temp += speed;
            transform.position = Vector3.Lerp(transform.position, Wakgood.Instance.transform.position, temp);
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        RuntimeManager.PlayOneShot("event:/SFX/ETC/LootGameObject", transform.position);
        OnEquip();
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    protected abstract void OnEquip();
}
