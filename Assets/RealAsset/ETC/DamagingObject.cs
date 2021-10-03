using UnityEngine;

public interface IDamagable
{
    void ReceiveDamage(int damage);
}

public class DamagingObject : MonoBehaviour
{
    [SerializeField] private bool canDamageWakgood;
    [SerializeField] private bool canDamageMonster;
    [SerializeField] private TotalAD totalAD;
    [SerializeField] private IntVariable criticalChance;
    [SerializeField] private int damage;
    private IDamagable damagable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") != canDamageWakgood || (other.CompareTag("Monster") || other.CompareTag("Boss")) != canDamageMonster) return;
        if (other.TryGetComponent(out damagable))
        {
            if ((other.CompareTag("Monster") || other.CompareTag("Boss")))
            {
                int damage;
                TextType textType;

                if (Random.Range(0, 100) < criticalChance.RuntimeValue)
                {
                    damage = (int)(totalAD.GetTotalDamage() * 1.5f);
                    textType = TextType.Critical;
                }
                else
                {
                    damage = totalAD.GetTotalDamage();
                    textType = TextType.Normal;
                }

                ObjectManager.Instance.PopObject("DamageText", other.transform).GetComponent<AnimatedText>().SetText(damage.ToString(), textType);
                damagable.ReceiveDamage(damage);
            }
            else
            {
                damagable.ReceiveDamage(damage);
            }       
        }
    }
}
