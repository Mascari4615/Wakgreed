using FMODUnity;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Skill/MeleeAttack")]
public class MeleeAttack : Skill
{
    [SerializeField] private float radius = 1;
    private static readonly int attack = Animator.StringToHash("Attack");

    public override void Use()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(Wakgood.Instance.AttackPosition.position, radius, Vector2.zero, 0, 1 << 7);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.TryGetComponent(out IHitable mob))
            {
                mob.ReceiveHit(Random.Range(Wakgood.Instance.CurWeapon.minDamage,
                    Wakgood.Instance.CurWeapon.maxDamage));
            }
        }

        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true);
        Wakgood.Instance.WeaponPosition.GetChild(0).GetComponent<Animator>()?.SetTrigger(attack);
        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{Wakgood.Instance.CurWeapon.id}");
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse();
    }
}
