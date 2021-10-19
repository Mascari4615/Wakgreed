using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Skill/RangedAttack")]
public class RangedAttack : Skill
{
    public override void Use()
    {
        RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack");
        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.attackPosition, true);
        Wakgood.Instance.curWeapon.ammo--;
    }
}
