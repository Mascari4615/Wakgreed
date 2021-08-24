using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLine : MonoBehaviour
{
    private WaitForSeconds waitForSeconds01 = new(0.1f);
    private Vector3 EndPosition;

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        GetComponent<TrailRenderer>().Clear();
        foreach (RaycastHit2D hitObject in Physics2D.RaycastAll(transform.position, TravellerController.Instance.transform.position - transform.position))
        {
            if (hitObject.transform.CompareTag("Wall"))
            {
                EndPosition = hitObject.point;
                break;
            }
        }

        StartCoroutine("Move");
    }

    private IEnumerator Move()
    {
        float duration = 1f;
        while (true)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                gameObject.SetActive(false);
            }

            transform.position = Vector3.Lerp(transform.position, EndPosition, Time.deltaTime * 5);
            yield return null;
        }
    }
}
