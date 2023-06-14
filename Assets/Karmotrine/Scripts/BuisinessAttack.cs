using UnityEngine;

public class BuisinessAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Wakgood.Instance.WakgoodMove.TryIced();
        }
    }
}