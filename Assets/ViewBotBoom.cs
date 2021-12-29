using UnityEngine;

public class ViewBotBoom : MonoBehaviour
{
    [SerializeField] private IntVariable viewer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            viewer.RuntimeValue += 100;
    }
}
