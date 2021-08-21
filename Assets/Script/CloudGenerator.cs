using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    // [SerializeField] private GameObject cloudGameObject;

    private void Awake()
    {
        StartCoroutine(CloudGenerate());
    }

    IEnumerator CloudGenerate()
    {
        Vector3 generatePos = new(60, Random.Range(-50f, 50f), 0);
        //ObjectManager.Instance.GetQueue(PoolType.Cloud, generatePos);
        ObjectManager.Instance.GetQueue("Cloud", generatePos);
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        StartCoroutine(CloudGenerate());
    }
}
