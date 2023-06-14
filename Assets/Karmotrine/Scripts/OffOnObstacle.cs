using UnityEngine;

public class OffOnObstacle : MonoBehaviour
{
    [SerializeField] private bool EffectOnDisable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            ObjectManager.Instance.PopObject("Effect_Hit", transform.position);
            gameObject.SetActive(false);
        }
    }
}