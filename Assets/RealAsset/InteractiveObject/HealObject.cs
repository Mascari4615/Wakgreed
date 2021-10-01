using FMODUnity;
using UnityEngine;

public class HealObject : MonoBehaviour
{
    [SerializeField] private int healAmount;
    [SerializeField] private IntVariable wakgoodCurHP;
    [SerializeField] private IntVariable wakgoodMaxHP;

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
        RuntimeManager.PlayOneShot("event:/SFX/ETC/Heal", Wakgood.Instance.attackPosition.position);
        wakgoodCurHP.RuntimeValue = Mathf.Clamp(wakgoodCurHP.RuntimeValue + healAmount, 0, wakgoodMaxHP.RuntimeValue);
    }
}
