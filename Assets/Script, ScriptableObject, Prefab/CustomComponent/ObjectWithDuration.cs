using UnityEngine;
using System.Collections;

public class ObjectWithDuration : MonoBehaviour
{
    [SerializeField] private float duration;
    private WaitForSeconds waitForSeconds;

    protected virtual void Awake()
    {
        // Debug.Log($"{name} : Awake");
        waitForSeconds = new WaitForSeconds(duration);
        // Debug.Log($"{name} : duration = {duration}, waitForSeconds = {waitForSeconds}");
    }

    protected virtual void OnEnable()
    {
        // Debug.Log($"{name} : OnEnable");
        StartCoroutine(CheckDuration());
    }

    private void OnDisable()
    {
        // Debug.Log($"{name} : OnDisable");
        // Prepare To Be Disabled Form OutSide
        StopCoroutine(CheckDuration());
    }

    private IEnumerator CheckDuration()
    {
        // Debug.Log($"{name} : CheckDurationStart");
        yield return waitForSeconds;
        gameObject.SetActive(false);
        // Debug.Log($"{name} : CheckDurationEnd");
    }
}
