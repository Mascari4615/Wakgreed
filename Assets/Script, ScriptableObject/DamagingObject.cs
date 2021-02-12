using UnityEngine;

public class DamagingObject : MonoBehaviour
{
    private enum Target
    {
        Monster,
        Traveller
    }
    [SerializeField] private IntVariable travellerAD;
    [SerializeField] private IntVariable TravellerCriticalChance;
    [SerializeField] private int monsterAD;
    [SerializeField] private Target target;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Monster" || other.tag == "Boss") && target == Target.Monster)
        {
            if (other.gameObject.TryGetComponent<Monster>(out Monster Monster))
            {
                int damage = travellerAD.RuntimeValue;
                string type = "";
                if (Random.Range(0, 100) < TravellerCriticalChance.RuntimeValue)
                {
                    damage *= 4;
                    type = "Critical";
                }
                Monster.ReceiveDamage(damage, type);
            }
        }
        else if (other.tag == "Player" && target == Target.Traveller)
        {
            if (other.gameObject.TryGetComponent<TravellerController>(out TravellerController t))
            {
                t.ReceiveDamage(monsterAD);
                gameObject.SetActive(false);
            }
        }
    }
}