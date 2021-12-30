using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Skill/RangedAttack")]
public class RangedAttack : Skill
{
    private Animator animator;
    public override void Use(Weapon weapon)
    {
        if (type.Equals(SkillType.Base))
        {
            if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[53]))
            {
                int per = 5 * DataManager.Instance.wgItemInven.itemCountDic[53];
                if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[52]))
                    per += 3 * DataManager.Instance.wgItemInven.itemCountDic[52];
                if (Random.Range(0, 100) < per)
                    ObjectManager.Instance.PopObject("Ball", Wakgood.Instance.transform.position).GetComponent<BulletMove>().SetDirection((Vector3)Wakgood.Instance.worldMousePoint - Wakgood.Instance.transform.position);
            }
        }


        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{weapon.id}");
        if (ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true) == null)
        {
            ObjectManager.Instance.AddPool(resource);
        }
        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true);
        Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].Ammo--;
        if (Wakgood.Instance.WeaponPosition.GetChild(0).TryGetComponent(out animator))
            animator.SetTrigger("Attack");
    }
}
