using System.Collections;
using UnityEngine;

public class ObjectWithDuration : MonoBehaviour
{
    [SerializeField] private float duration;
    private WaitForSeconds waitForSeconds;
    private bool isPoolingObject;

    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(duration);
        isPoolingObject = GetComponent<PoolingObject>();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckDuration());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator CheckDuration()
    {
        yield return waitForSeconds;

        if (isPoolingObject)
        {
            ObjectManager.Instance.SetObjectParent(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
