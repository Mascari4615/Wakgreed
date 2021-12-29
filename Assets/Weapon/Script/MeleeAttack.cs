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
        if (type.Equals(SkillType.Base))
        {
            
            if (DataManager.Instance.wakgoodItemInventory.Items.Contains(DataManager.Instance.ItemDic[53]))
            {
                int per = 5 * DataManager.Instance.wakgoodItemInventory.itemCountDic[53];
                if (DataManager.Instance.wakgoodItemInventory.Items.Contains(DataManager.Instance.ItemDic[52]))
                    per += 3 * DataManager.Instance.wakgoodItemInventory.itemCountDic[52];
                if (Random.Range(0, 100) < per)
                    ObjectManager.Instance.PopObject("Ball", Wakgood.Instance.transform.position).GetComponent<BulletMove>().SetDirection((Vector3)Wakgood.Instance.worldMousePoint - Wakgood.Instance.transform.position);
            }
        }

        if (ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true) == null)
        {
            ObjectManager.Instance.AddPool(resource);
        }
        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(Wakgood.Instance.AttackPosition.position, radius, Vector2.zero, 0, 1 << 7);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Monster") || hit.transform.CompareTag("Boss"))
            {
                if (hit.transform.TryGetComponent(out IHitable mob))
                {
                    mob.ReceiveHit((int)Math.Round(
                        Random.Range(weapon.minDamage, weapon.maxDamage) *
                        (1 + (float)Wakgood.Instance.totalPower.RuntimeValue / 100), MidpointRounding.AwayFromZero));
                }
            }
        }

        if (Wakgood.Instance.WeaponPosition.GetChild(0).TryGetComponent(out animator))
            animator.SetTrigger("Attack");

        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{weapon.id}");
        GameManager.Instance.CinemachineImpulseSource.GenerateImpulse();
    }
}
