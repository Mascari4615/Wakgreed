using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Skill/RangedAttack")]
public class RangedAttack : Skill
{
    public override void Use(Weapon weapon)
    {
        RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack");
        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true);
        Wakgood.Instance.CurWeapon.Ammo--;
    }
}
