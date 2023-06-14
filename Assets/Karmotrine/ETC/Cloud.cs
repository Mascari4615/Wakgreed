using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private float minSize = 1f;
    [SerializeField] private int maxA = 67;
    [SerializeField] private int minA = 67;
    [SerializeField] private float speedA = 1;
    private float size;
    private float speed;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.position -= new Vector3(1, 0, 0) * Time.deltaTime * speed;
        if (transform.position.x <= -100)
        {
            if (ObjectManager.Instance != null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnEnable()
    {
        speed = Random.Range(1f, 5f) * speedA;
        size = Random.Range(minSize, maxSize);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (float)Random.Range(minA, maxA + 1) / 255);

        transform.localScale = Vector3.one * size;
    }
}