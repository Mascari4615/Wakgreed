using UnityEngine;

public interface Damagable
{
    void ReceiveDamage(int damage);
}

public class DamagingObject : MonoBehaviour
{
    private enum Target
    {
        Monster,
        Traveller
    }
    [SerializeField] private TotalAD totalAD;
    [SerializeField] private IntVariable TravellerCriticalChance;
    [SerializeField] private int monsterAD;
    [SerializeField] private Target target;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Monster") || other.CompareTag("Boss")) && target.Equals(Target.Monster))
        {      
            int damage;
            TextType textType;

            if (Random.Range(0, 100) < TravellerCriticalChance.RuntimeValue)
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
            other.gameObject.GetComponent<Monster>().ReceiveDamage(damage);
        }
        else if (other.CompareTag("Player") && target.Equals(Target.Traveller))
        {
            other.gameObject.GetComponent<Wakgood>().ReceiveDamage(monsterAD);
        }
        
        if (other.CompareTag("Box"))
        {
            other.GetComponent<Box>().ReceiveDamage(0);
        }
    }
}
