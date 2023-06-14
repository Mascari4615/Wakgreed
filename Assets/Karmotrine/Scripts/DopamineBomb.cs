using System.Collections;
using UnityEngine;

public class DopamineBomb : MonoBehaviour
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

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator GO()
    {
        yield return new WaitForSeconds(.5f);
        warningObject.SetActive(true);
        yield return new WaitForSeconds(.5f);

        drop.transform.localPosition = Vector3.up * 15;
        drop.SetActive(true);
        for (float i = 0; i <= 1; i += Time.deltaTime * 1.3f)
        {
            drop.transform.localPosition = Vector3.Lerp(drop.transform.localPosition, Vector3.zero, i);
            if (Vector3.Distance(drop.transform.localPosition, Vector3.zero) < .3f)
            {
                break;
            }

            yield return null;
        }

        drop.transform.localPosition = Vector3.zero;
        GameManager.Instance.CinemachineImpulseSource.GenerateImpulse(10f);
        ObjectManager.Instance.PopObject("LevelUpEffect", transform);

        warningObject.SetActive(false);

        damagingObject.enabled = true;
        yield return new WaitForSeconds(.2f);
        damagingObject.enabled = false;

        yield return new WaitForSeconds(2f);
        ObjectManager.Instance.PopObject("Effect_Bomb", transform);
        ObjectManager.Instance.PopObject("Banshee", transform.position + ((Vector3)Random.insideUnitCircle * 3));
        ObjectManager.Instance.PopObject("ViewBot", transform.position + ((Vector3)Random.insideUnitCircle * 3));
        gameObject.SetActive(false);
    }
}