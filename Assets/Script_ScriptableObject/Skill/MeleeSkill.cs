using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "MeleeSkill", menuName = "Skill/MeleeSkill")]
public class MeleeSkill : Skill
{
    public override void Use()
    {
        ObjectManager.Instance.PopObject(resource.name, TravellerController.Instance.attackPosition);
        if (TravellerController.Instance.weaponPosition.GetChild(0).GetComponent<Animator>() != null)
            TravellerController.Instance.weaponPosition.GetChild(0).GetComponent<Animator>()?.SetTrigger("Attack");
        RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack", TravellerController.Instance.attackPosition.position);
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse();
    }
}
