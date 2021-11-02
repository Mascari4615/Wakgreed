using System.Collections;
using UnityEngine;

public class ObjectWithDuration : MonoBehaviour
{
    [SerializeField] private float duration;
    private WaitForSeconds waitForSeconds;

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
        gameObject.SetActive(false);
    }
}
