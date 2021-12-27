using System;
using UnityEngine;
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
        if ((other.CompareTag("Player") != canDamageWakgood) || ((other.CompareTag("Monster") || other.CompareTag("Boss"))) != canDamageMob) 
            return;

        if (other.TryGetComponent(out IHitable damageable))
        {
            if (other.CompareTag("Monster") || other.CompareTag("Boss"))
            {
                int totalDamage = WeaponID != -1 ? UnityEngine.Random.Range(minDamage, maxDamage + 1) : damage;
                TextType textType;

                if (UnityEngine.Random.Range(0, 100) < criticalChance.RuntimeValue)
                {
                    totalDamage = (int) Math.Round(totalDamage * totalPower.RuntimeValue * (1 + criticalDamagePer.RuntimeValue * 0.01f), MidpointRounding.AwayFromZero);
                    textType = TextType.Critical;
                }
                else
                {
                    totalDamage *= (1 + totalPower.RuntimeValue / 100);
                    textType = TextType.Normal;
                }
                ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText(damage.ToString(), textType);

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

        if (offGoOnHit)
            gameObject.SetActive(false);
        else if (offCollOnHit)
            collider2D.enabled = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
