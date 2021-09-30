using FMODUnity;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "MeleeSkill", menuName = "Skill/MeleeSkill")]
public class MeleeSkill : Skill
{
    public override void Use()
    {
        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.attackPosition);
        if (Wakgood.Instance.weaponPosition.GetChild(0).GetComponent<Animator>() != null)
            Wakgood.Instance.weaponPosition.GetChild(0).GetComponent<Animator>().SetTrigger("Attack");
        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{Wakgood.Instance.curWeapon.ID}", Wakgood.Instance.attackPosition.position);
        string path = $"event:/SFX/Weapon/{Wakgood.Instance.curWeapon.ID}";
        if ((from event_ in EventManager.Events where event_.Path.Equals(path) select event_).Count() == 0)
            RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack", Wakgood.Instance.attackPosition.position);
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse();
    }
}
