using System.Collections;
using UnityEngine;

public class NeckPlaneGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cloud;
    [SerializeField] private float xParameter = 5;
    [SerializeField] private float y = 60;
    [SerializeField] private float durationParameter = 5;

    private IEnumerator Start()
    {
        while (true)
        {
            Vector3 generatePos = new(Random.Range(-10f, 10f) * xParameter, y, 0);
            Instantiate(cloud, generatePos, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(1f, 2f) * durationParameter);
        }
    }
}