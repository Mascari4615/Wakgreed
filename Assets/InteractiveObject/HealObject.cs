using FMODUnity;
using UnityEngine;

public class HealObject : MonoBehaviour
{
    [SerializeField] private int healAmount;
    [SerializeField] private IntVariable HpCur;
    [SerializeField] private MaxHp HpMax;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (HpCur.RuntimeValue < HpMax.RuntimeValue)
            {
                RuntimeManager.PlayOneShot("event:/SFX/ETC/Heal", Wakgood.Instance.AttackPosition.position);
                Wakgood.Instance.ReceiveHeal(healAmount);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (HpCur.RuntimeValue < HpMax.RuntimeValue)
            {
                RuntimeManager.PlayOneShot("event:/SFX/ETC/Heal", Wakgood.Instance.AttackPosition.position);
                Wakgood.Instance.ReceiveHeal(healAmount);
                gameObject.SetActive(false);
            }
        }
    }
}
