using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public interface IHitable
{
    void ReceiveHit(int damage);
}

public class DamagingObject : MonoBehaviour
{
    [SerializeField] private bool canDamageWakgood;
    [FormerlySerializedAs("canDamageMonster")] [SerializeField] private bool canDamageMob;
    [SerializeField] private TotalPower totalPower;
    [SerializeField] private IntVariable criticalChance;
    [SerializeField] private IntVariable criticalDamagePer;
    [SerializeField] private int damage = 0;
    [SerializeField] private bool offOnHit = true;
    [SerializeField] [Range(0f, 100f)] private float ggambbakDelay = 0f;
    private WaitForSeconds wsDelay;
    private new Collider2D collider2D;
    private IHitable damageable;

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        wsDelay = new WaitForSeconds(ggambbakDelay);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") != canDamageWakgood) || ((other.CompareTag("Monster") || other.CompareTag("Boss"))) != canDamageMob) 
            return;

        if (other.TryGetComponent(out damageable))
        {
            if ((other.CompareTag("Monster") || other.CompareTag("Boss")))
            {
                int totalDamage;
                // TextType textType;

                if (UnityEngine.Random.Range(0, 100) < criticalChance.RuntimeValue)
                {
                    totalDamage = (int) Math.Round(damage * totalPower.RuntimeValue * (1 + criticalDamagePer.RuntimeValue * 0.01f), MidpointRounding.AwayFromZero);
                    // textType = TextType.Critical;
                }
                else
                {
                    totalDamage = damage * totalPower.RuntimeValue;
                    // textType = TextType.Normal;
                }

                damageable.ReceiveHit(totalDamage);
            }
            else
            {
                damageable.ReceiveHit(damage);
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
