using UnityEngine;

public abstract class Loot : MonoBehaviour
{
    private Vector3 waitPosition;
    protected float waitTime = 0.5f;
    private float currentWaitTime = 0.5f;
    private float waitMoveSpeed = 0;
    private float moveSpeed = 0;
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
        waitPosition = transform.position + new Vector3(Random.Range(-1f, 2f), Random.Range(-1f, 2f), 0);
        circleCollider2D.enabled = false;
        currentWaitTime = waitTime;
        waitMoveSpeed = 0;
        moveSpeed = 0;

        _OnEnable();
    }

    protected virtual void _OnEnable() {}

    private void Update()
    {
        if (currentWaitTime > 0)
        {
            waitMoveSpeed += Time.deltaTime;
            currentWaitTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, waitPosition, waitMoveSpeed * 0.6f); 
        }
        else
        {
            moveSpeed += Time.deltaTime;
            circleCollider2D.enabled = true;
            transform.position = Vector3.Lerp(transform.position, TravellerController.Instance.transform.position, moveSpeed); 
        }      
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnEquip();
            
            if (!(gameEvent is null)) gameEvent.Raise();
            gameObject.SetActive(false);
        }
    }

    protected abstract void OnEquip();
}
