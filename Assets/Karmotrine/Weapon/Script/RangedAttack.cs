using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "Skill/RangedAttack")]
public class RangedAttack : Skill
{
    [SerializeField] private bool setRot = true;
    private Animator animator;

    public override void Use(Weapon weapon)
    {
        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{weapon.id}");
        GameManager.Instance.CinemachineImpulseSource.GenerateImpulse();

        if (ObjectManager.Instance.CheckPool(resource.name) == false)
        {
            ObjectManager.Instance.AddPool(resource);
        }

        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, setRot);
        Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].Ammo--;
        if (Wakgood.Instance.WeaponPosition.GetChild(0).TryGetComponent(out animator))
        {
            animator.SetTrigger("Attack");
        }
    }
}