using UnityEngine;

public class OffOnObstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            gameObject.SetActive(false);
        }
    }
}
