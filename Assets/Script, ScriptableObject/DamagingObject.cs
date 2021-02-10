using UnityEngine;

public class DamagingObject : MonoBehaviour
{
    private enum Target
    {
        Monster,
        Traveller
    }
    [SerializeField] private IntVariable travellerAD;
    [SerializeField] private int monsterAD;
    [SerializeField] private Target target;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster" && target == Target.Monster)
        {
            if (other.gameObject.TryGetComponent<Monster>(out Monster Monster))
            {
                Monster.ReceiveDamage(travellerAD.RuntimeValue);
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