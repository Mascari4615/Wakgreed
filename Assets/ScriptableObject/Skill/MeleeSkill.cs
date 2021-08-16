using UnityEngine;

[CreateAssetMenu(fileName = "MeleeSkill", menuName = "Skill/MeleeSkill")]
public class MeleeSkill : Skill
{
    public override void Attack(TravellerController parent)
    {
        ObjectManager.Instance.GetQueue(resource.name, parent.attackPosition);
        parent.weaponPosition.GetChild(0).GetComponent<Animator>().SetTrigger("Attack");
        AudioManager.Instance.PlayAudioClip(parent.curWeapon.soundEffect);
    }
}
