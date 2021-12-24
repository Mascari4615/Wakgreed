using FMODUnity;
using UnityEngine;

public class HealObject : MonoBehaviour
{
    [SerializeField] private int healAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Heal();
            gameObject.SetActive(false);
        }
    }

    private void Heal()
    {
        RuntimeManager.PlayOneShot("event:/SFX/ETC/Heal", Wakgood.Instance.AttackPosition.position);
        Wakgood.Instance.ReceiveHeal(healAmount);
    }
}
