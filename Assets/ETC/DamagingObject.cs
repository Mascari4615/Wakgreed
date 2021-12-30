using System;
using UnityEngine;

public enum HitType
{
    Normal,
    Critical
}

public interface IHitable
{
    void ReceiveHit(int damage, HitType hitType = HitType.Normal);
}

public class DamagingObject : MonoBehaviour
{
    [SerializeField] private bool bTargetWak = true;

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

    private void OnEnable()
    {
        collider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Player") && bTargetWak == false) ||
            (other.CompareTag("Monster") || other.CompareTag("Boss")) && bTargetWak == true)
            return;

        if ((other.CompareTag("Monster") || other.CompareTag("Boss")) && bTargetWak == false)
        {
            if (other.TryGetComponent(out IHitable damageable))
            {
                Debug.Log(other);

                if (UnityEngine.Random.Range(0, 100) < Wakgood.Instance.miss.RuntimeValue)
                {
                    ObjectManager.Instance.PopObject("AnimatedText", other.transform.position + Vector3.up).GetComponent<AnimatedText>().SetText("ºø³ª°¨!", Color.red);

                    if (offGoOnHit)
                    {
                        gameObject.SetActive(false);
                    }
                    else if (offCollOnHit)
                    {
                        collider2D.enabled = false;
                    }

                    return;
                }

                int totalDamage = WeaponID != -1 ? UnityEngine.Random.Range(minDamage, maxDamage + 1) : damage;

                HitType hitType = HitType.Normal;
                totalDamage = (int)Math.Round(totalDamage * (1 + (float)Wakgood.Instance.totalPower.RuntimeValue / 100));
                if (UnityEngine.Random.Range(0, 100) < Wakgood.Instance.criticalChance.RuntimeValue)
                {
                    totalDamage = (int)Math.Round(totalDamage * (1.2f + (float)Wakgood.Instance.criticalDamagePer.RuntimeValue / 100), MidpointRounding.AwayFromZero);
                    hitType = HitType.Critical;
                }

                if (other.CompareTag("Monster"))
                {
                    totalDamage = (int)Math.Round(totalDamage * (1 + (float)Wakgood.Instance.MobDamage.RuntimeValue / 100), MidpointRounding.AwayFromZero);
                }
                else if (other.CompareTag("Boss"))
                {
                    totalDamage = (int)Math.Round(totalDamage * (1 + (float)Wakgood.Instance.BossDamage.RuntimeValue / 100), MidpointRounding.AwayFromZero);
                }
                damageable.ReceiveHit(totalDamage, hitType);
                if (offGoOnHit)
                {
                    gameObject.SetActive(false);
                }
                else if (offCollOnHit)
                {
                    collider2D.enabled = false;
                }
            }
        }
        else if (other.CompareTag("Player") && bTargetWak == true)
        {
            // Debug.Log($"{this.gameObject.name}, => {other.gameObject.name}");
            if (other.transform.parent.TryGetComponent(out IHitable wakgood))
            {
                wakgood.ReceiveHit(damage);
                if (offGoOnHit)
                {
                    gameObject.SetActive(false);
                }
                else if (offCollOnHit)
                {
                    collider2D.enabled = false;
                }
            }
        }  
    }
}
