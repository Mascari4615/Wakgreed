using System.Collections;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cloud;
    [SerializeField] private float yParameter = 5;
    [SerializeField] private float x = 60;
    [SerializeField] private float durationParameter = 5;
    public static CloudGenerator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            Vector3 generatePos = new(x, Random.Range(-10f, 10f) * yParameter, 0);

            if (ObjectManager.Instance != null)
            {
                ObjectManager.Instance.PopObject("Cloud", generatePos);
            }
            else
            {
                Instantiate(cloud, generatePos, Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(1f, 2f) * durationParameter);
        }
    }
}