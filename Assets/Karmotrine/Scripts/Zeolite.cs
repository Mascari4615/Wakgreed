using System.Collections;
using UnityEngine;

public class Zeolite : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine("GO");
    }

    private IEnumerator GO()
    {
        float t = 0;
        Vector3 origin = transform.position;
        transform.position = transform.position + (Vector3.up * 15);
        while (Vector3.Distance(transform.position, origin) > .5f)
        {
            yield return null;
            transform.position = Vector3.Lerp(transform.position, origin, t += Time.deltaTime);
        }

        GameManager.Instance.CinemachineImpulseSource.GenerateImpulse(30f);

        transform.position = origin;
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}