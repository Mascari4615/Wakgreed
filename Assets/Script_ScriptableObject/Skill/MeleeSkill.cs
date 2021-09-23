using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeSkill", menuName = "Skill/MeleeSkill")]
public class MeleeSkill : Skill
{
    public override void Use()
    {
        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.attackPosition);
        if (Wakgood.Instance.weaponPosition.GetChild(0).GetComponent<Animator>() != null)
            Wakgood.Instance.weaponPosition.GetChild(0).GetComponent<Animator>().SetTrigger("Attack");
        RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack", Wakgood.Instance.attackPosition.position);
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse();
    }
}
