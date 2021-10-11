using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Skill/RangedAttack")]
public class RangedAttack : Skill
{
    public override void Use(int minDamage, int maxDamage)
    {
        if (Wakgood.Instance.curWeapon.ammo > 0)
        {
            RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack", Wakgood.Instance.attackPosition.position);

            if (resource != null)
            {
                ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.attackPosition);
            }
            else
            {
                //ObjectManager.Instance.GetQueue(PoolType.KnightSwordSlash, parent.attackPosition);
                ObjectManager.Instance.PopObject("Arrow", Wakgood.Instance.attackPosition);
            }

            Wakgood.Instance.curWeapon.ammo--;
            Debug.Log("CurAmmo : " + Wakgood.Instance.curWeapon.ammo);
        }
    }
}
