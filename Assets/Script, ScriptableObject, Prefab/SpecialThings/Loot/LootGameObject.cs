using UnityEngine;
using System.Collections;

public abstract class LootGameObject : MonoBehaviour
{
    private Vector3 waitPosition;
<<<<<<< HEAD:Assets/Script, ScriptableObject, Prefab/SpecialThings/Loot/LootGameObject.cs
    private float currentWaitTime;
=======
>>>>>>> 7195055b16ed9a40fd6ac9cc5ddf829d30020a9f:Assets/Script, ScriptableObject, Prefab/SpecialThings/Loot/Loot.cs
    private float waitMoveSpeed;
    private float moveSpeed;
    private CircleCollider2D circleCollider2D;
    private AudioSource audioSource;
    protected SpriteRenderer spriteRenderer;
    [SerializeField] private GameEvent gameEvent;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        waitPosition = transform.position + new Vector3(Random.Range(-1.5f, 1.6f), Random.Range(-1.5f, 1.6f), 0);
        circleCollider2D.enabled = false;
<<<<<<< HEAD:Assets/Script, ScriptableObject, Prefab/SpecialThings/Loot/LootGameObject.cs
        currentWaitTime = Random.Range(0.5f, 1.1f);
        waitMoveSpeed = 0;
        moveSpeed = 0;
    }

    private void Update()
=======
        waitMoveSpeed = 0;
        moveSpeed = 0;

        _OnEnable();

        StartCoroutine(Move());
    }

    protected virtual void _OnEnable() {}

    private IEnumerator Move()
>>>>>>> 7195055b16ed9a40fd6ac9cc5ddf829d30020a9f:Assets/Script, ScriptableObject, Prefab/SpecialThings/Loot/Loot.cs
    {
        while (waitMoveSpeed < 1)
        {
<<<<<<< HEAD:Assets/Script, ScriptableObject, Prefab/SpecialThings/Loot/LootGameObject.cs
            waitMoveSpeed += Time.deltaTime;
            currentWaitTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, waitPosition, waitMoveSpeed * 0.4f); 
=======
            waitMoveSpeed += 0.02f;
            transform.position = Vector3.Lerp(transform.position, waitPosition, waitMoveSpeed);
            yield return new WaitForSeconds(0.02f);
>>>>>>> 7195055b16ed9a40fd6ac9cc5ddf829d30020a9f:Assets/Script, ScriptableObject, Prefab/SpecialThings/Loot/Loot.cs
        }

        circleCollider2D.enabled = true;

        while (moveSpeed < 1)
        {
            moveSpeed += 0.02f;
            transform.position = Vector3.Lerp(transform.position, TravellerController.Instance.transform.position, moveSpeed);
            yield return new WaitForSeconds(0.02f);
        }   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log($"{name} : OnEquip");
            OnEquip();
            
            // Debug.Log($"{name} : OnEquip 2 Raise");
            if (!(gameEvent is null)) gameEvent.Raise();
            gameObject.SetActive(false);
        }
    }

    protected abstract void OnEquip();
}
