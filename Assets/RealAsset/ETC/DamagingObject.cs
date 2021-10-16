using UnityEngine;
using System.Collections;

public interface IHitable
{
    void ReceiveHit(int damage);
}

public class DamagingObject : MonoBehaviour
{
    [SerializeField] private bool canDamageWakgood;
    [SerializeField] private bool canDamageMonster;
    [SerializeField] private TotalAD totalAD;
    [SerializeField] private IntVariable criticalChance;
    [SerializeField] private int damage = 0;
    [SerializeField] private bool offOnHit = true;
    [SerializeField] [Range(0f, 100f)] private float ggambbakDelay = 0f;
    private WaitForSeconds wsDelay;
    private new Collider2D collider2D;
    private IHitable damagable;

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        wsDelay = new WaitForSeconds(ggambbakDelay);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") != canDamageWakgood) || ((other.CompareTag("Monster") || other.CompareTag("Boss"))) != canDamageMonster) 
            return;

        if (other.TryGetComponent(out damagable))
        {
            if ((other.CompareTag("Monster") || other.CompareTag("Boss")))
            {
                int totalDamage;
                // TextType textType;

                if (Random.Range(0, 100) < criticalChance.RuntimeValue)
                {
                    totalDamage = (int)((damage.Equals(0) ? totalAD.GetTotalDamage() : damage) * 1.5f);
                    // textType = TextType.Critical;
                }
                else
                {
                    totalDamage = damage.Equals(0) ? totalAD.GetTotalDamage() : damage;
                    // textType = TextType.Normal;
                }

                damagable.ReceiveHit(totalDamage);
            }
            else
            {
                damagable.ReceiveHit(damage);
            }     
        }
        else
        {
            Wakgood.Instance.ReceiveHit(damage);
        }

        if (offOnHit)
        {
            gameObject.SetActive(false);
        }
        else if (ggambbakDelay != 0)
        {
            StartCoroutine(Test());
        }
    }

    private IEnumerator Test()
    {
        collider2D.enabled = false;
        yield return wsDelay;
        collider2D.enabled = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
