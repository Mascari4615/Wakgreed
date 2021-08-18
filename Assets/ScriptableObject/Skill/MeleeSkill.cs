using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "MeleeSkill", menuName = "Skill/MeleeSkill")]
public class MeleeSkill : Skill
{
    public override void Attack(TravellerController parent)
    {
        ObjectManager.Instance.GetQueue(resource.name, parent.attackPosition);
        if (parent.weaponPosition.GetChild(0).GetComponent<Animator>() != null)
        parent.weaponPosition.GetChild(0).GetComponent<Animator>()?.SetTrigger("Attack");
        RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack", parent.attackPosition.position);
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse();
    }
}
