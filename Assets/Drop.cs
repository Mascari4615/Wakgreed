using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] private Collider2D damagingObject;
    [SerializeField] private GameObject warningObject;
    [SerializeField] private GameObject drop;

    private void OnEnable()
    {
        drop.SetActive(false);
        warningObject.SetActive(false);
        damagingObject.enabled = false;
        StartCoroutine("GO");
    }

    private IEnumerator GO()
    {
        yield return new WaitForSeconds(1f);
        warningObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        drop.transform.localPosition = Vector3.up * 15;
        drop.SetActive(true);
        for (float i = 0; i <= 1; i += Time.deltaTime)  
        {
            drop.transform.localPosition = Vector3.Lerp(drop.transform.localPosition, Vector3.zero, i);
            if (Vector3.Distance(drop.transform.localPosition, Vector3.zero) < .3f)
            {
                break;
            }

                yield return null;
        }
        drop.transform.localPosition = Vector3.zero;
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse(10f);
        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        warningObject.SetActive(false);

        damagingObject.enabled = true;
        yield return new WaitForSeconds(.2f);
        damagingObject.enabled = false;

        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
