using System;
using UnityEngine;
using UnityEngine.Serialization;

public interface IHitable
{
    void ReceiveHit(int damage);
}

public class DamagingObject : MonoBehaviour
{
    [SerializeField] private bool targetWak;
    [SerializeField] private TotalPower totalPower;
    [SerializeField] private IntVariable criticalChance;
    [SerializeField] private IntVariable criticalDamagePer;
    public int damage = 0;
    [SerializeField] private int WeaponID = -1;
    private int minDamage = -1;
    private int maxDamage = -1;
    [SerializeField] private bool offGoOnHit = false;
    [SerializeField] private bool offCollOnHit = false;
    private new Collider2D collider2D;

    private void Awake()
    {
        if (WeaponID != -1)
        {
            minDamage = DataManager.Instance.WeaponDic[WeaponID].minDamage;
            maxDamage = DataManager.Instance.WeaponDic[WeaponID].maxDamage;
        }
        collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") && targetWak == false) ||
            (other.CompareTag("Monster") || other.CompareTag("Boss")) && targetWak == true) 
            return;

        if (other.TryGetComponent(out IHitable damageable))
        {
            if (other.CompareTag("Monster") || other.CompareTag("Boss"))
            {
                int totalDamage = WeaponID != -1 ? UnityEngine.Random.Range(minDamage, maxDamage + 1) : damage;

                if (criticalChance == null)
                {
                    damageable.ReceiveHit(totalDamage);
                }
                else
                {
                    totalDamage = UnityEngine.Random.Range(0, 100) < criticalChance.RuntimeValue
                        ? (int)Math.Round(totalDamage * totalPower.RuntimeValue * (1 + criticalDamagePer.RuntimeValue * 0.01f), MidpointRounding.AwayFromZero)
                        : (int)Math.Round(totalDamage * (1 + (float)totalPower.RuntimeValue / 100), MidpointRounding.AwayFromZero);

                    if (other.CompareTag("Monster"))
                    {
                        totalDamage = (int)Math.Round(totalDamage * (1 + (float)Wakgood.Instance.MobDamage.RuntimeValue / 100), MidpointRounding.AwayFromZero);
                    }
                    else if (other.CompareTag("Boss"))
                    {
                        totalDamage = (int)Math.Round(totalDamage * (1 + (float)Wakgood.Instance.BossDamage.RuntimeValue / 100), MidpointRounding.AwayFromZero);
                    }
                    damageable.ReceiveHit(totalDamage);
                }
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

        if (offGoOnHit)
            gameObject.SetActive(false);
        else if (offCollOnHit)
            collider2D.enabled = false;
    }
}
