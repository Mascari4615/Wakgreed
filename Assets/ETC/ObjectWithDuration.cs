using System.Collections;
using UnityEngine;

public class ObjectWithDuration : MonoBehaviour
{
    [SerializeField] private float duration;
    private WaitForSeconds waitForSeconds;
    private PoolingObject poolingObject;

    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(duration);
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
        if (TryGetComponent(out poolingObject))
        {
            ObjectManager.Instance.SetObjectParent(gameObject);
        }
        gameObject.SetActive(false);     
    }
}
