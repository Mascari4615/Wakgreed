using System.Collections;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CloudGenerate());
    }

    private IEnumerator CloudGenerate()
    {
        Vector3 generatePos = new(60, Random.Range(-50f, 50f), 0);
        ObjectManager.Instance.PopObject("Cloud", generatePos);
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        StartCoroutine(CloudGenerate());
    }
}
