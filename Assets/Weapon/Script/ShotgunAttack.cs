using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunAttack", menuName = "Skill/ShotgunAttack")]
public class ShotgunAttack : Skill
{
    [SerializeField] private int count = 1;
    [SerializeField] private bool setRot = true;
    private Animator animator;

    public override void Use(Weapon weapon)
    {
        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{weapon.id}");
        Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].Ammo--;
        for (int i = 0; i < count; i++)
        {
            if (ObjectManager.Instance.CheckPool(resource.name) == false)
                ObjectManager.Instance.AddPool(resource);
            ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition.position + (Vector3)Random.insideUnitCircle.normalized * 0.4f, Wakgood.Instance.AttackPosition.rotation.eulerAngles);        
            if (Wakgood.Instance.WeaponPosition.GetChild(0).TryGetComponent(out animator))
                animator.SetTrigger("Attack");
        }       
    }
}
