using FMODUnity;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Skill/MeleeAttack")]
public class MeleeAttack : Skill
{
    [SerializeField] private float radius = 1;
    private Animator animator;

    public override void Use(Weapon weapon)
    {
        if (Wakgood.Instance.WeaponPosition.GetChild(0).TryGetComponent(out animator))
        {
            animator.SetTrigger("Attack");
        }

        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{weapon.id}");
        GameManager.Instance.CinemachineImpulseSource.GenerateImpulse();

        if (ObjectManager.Instance.CheckPool(resource.name) == false)
        {
            ObjectManager.Instance.AddPool(resource);
        }

        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true);

        RaycastHit2D[] hits =
            Physics2D.CircleCastAll(Wakgood.Instance.AttackPosition.position, radius, Vector2.zero, 0, 1 << 7);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Monster") || hit.transform.CompareTag("Boss"))
            {
                if (hit.transform.TryGetComponent(out IHitable damageable))
                {
                    if (Random.Range(0, 100) < Wakgood.Instance.miss.RuntimeValue)
                    {
                        ObjectManager.Instance.PopObject("AnimatedText", hit.transform.position + Vector3.up)
                            .GetComponent<AnimatedText>().SetText("빗나감!", Color.red);
                        return;
                    }

                    int totalDamage = Random.Range(weapon.minDamage, weapon.maxDamage + 1);

                    HitType hitType = HitType.Normal;
                    totalDamage =
                        (int)Math.Round(totalDamage * (1 + ((float)Wakgood.Instance.totalPower.RuntimeValue / 100)));
                    if (Random.Range(0, 100) < Wakgood.Instance.criticalChance.RuntimeValue)
                    {
                        totalDamage =
                            (int)Math.Round(
                                totalDamage * (1.2f + ((float)Wakgood.Instance.criticalDamagePer.RuntimeValue / 100)),
                                MidpointRounding.AwayFromZero);
                        hitType = HitType.Critical;
                    }

                    if (hit.transform.CompareTag("Monster"))
                    {
                        totalDamage =
                            (int)Math.Round(totalDamage * (1 + ((float)Wakgood.Instance.MobDamage.RuntimeValue / 100)),
                                MidpointRounding.AwayFromZero);
                    }
                    else if (hit.transform.CompareTag("Boss"))
                    {
                        totalDamage =
                            (int)Math.Round(totalDamage * (1 + ((float)Wakgood.Instance.BossDamage.RuntimeValue / 100)),
                                MidpointRounding.AwayFromZero);
                    }

                    damageable.ReceiveHit(totalDamage, hitType);
                }
            }
            else if (hit.transform.CompareTag("Box"))
            {
                if (hit.transform.TryGetComponent(out IHitable damageable))
                {
                    damageable.ReceiveHit(0);
                }
            }
        }
    }
}