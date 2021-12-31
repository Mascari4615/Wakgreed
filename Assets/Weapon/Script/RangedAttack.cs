using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Skill/RangedAttack")]
public class RangedAttack : Skill
{
    private Animator animator;

    public override void Use(Weapon weapon)
    {
        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{weapon.id}");
        if (ObjectManager.Instance.CheckPool(resource.name) == false)
            ObjectManager.Instance.AddPool(resource);
        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true);
        Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].Ammo--;
        if (Wakgood.Instance.WeaponPosition.GetChild(0).TryGetComponent(out animator))
            animator.SetTrigger("Attack");
    }
}
