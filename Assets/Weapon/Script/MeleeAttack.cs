using FMODUnity;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Skill/MeleeAttack")]
public class MeleeAttack : Skill
{
    [SerializeField] private float radius = 1;
    private static readonly int attack = Animator.StringToHash("Attack");

    public override void Use(Weapon weapon)
    {
        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true);
        
        RaycastHit2D[] hits = Physics2D.CircleCastAll(Wakgood.Instance.AttackPosition.position, radius, Vector2.zero, 0, 1 << 7);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.TryGetComponent(out IHitable mob))
            {
                mob.ReceiveHit((int)Math.Round(
                    Random.Range(weapon.minDamage, weapon.maxDamage) *
                    (1 + (float)Wakgood.Instance.totalPower.RuntimeValue / 100), MidpointRounding.AwayFromZero));
            }
        }

        Wakgood.Instance.WeaponPosition.GetChild(0).GetComponent<Animator>()?.SetTrigger(attack);
        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{weapon.id}");
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse();
    }
}
