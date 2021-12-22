using UnityEngine;

public class Cloud : MonoBehaviour
{
    private SpriteRenderer sr;
    private float speed;
    private float size;
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private float minSize = 1f;
    [SerializeField] private int maxA = 67;
    [SerializeField] private int minA = 67;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        speed = Random.Range(1f, 5f);
        size = Random.Range(minSize, maxSize);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (float)Random.Range(minA, maxA + 1) / 255);

        transform.localScale = Vector3.one * size;
    }

    private void Update()
    {
        transform.position -= new Vector3(1, 0, 0) * Time.deltaTime * speed;
        if (transform.position.x <= -60)
        {
            if (ObjectManager.Instance != null)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
    }
}
