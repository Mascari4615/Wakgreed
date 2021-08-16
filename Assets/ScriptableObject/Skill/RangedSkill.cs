using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "RangedSkill", menuName = "Skill/RangedSkill")]
public class RangedSkill : Skill, ISerializationCallbackReceiver
{
    static bool isReloading = false;

    public override void Attack(TravellerController parent)
    {
        if (isReloading) return;
        if (parent.curWeapon.ammo > 0)
        {
            AudioManager.Instance.PlayAudioClip(parent.curWeapon.soundEffect);

            if (resource != null)
            {
                ObjectManager.Instance.GetQueue(resource.name, parent.attackPosition);
            }
            else
            {
                //ObjectManager.Instance.GetQueue(PoolType.KnightSwordSlash, parent.attackPosition);
                ObjectManager.Instance.GetQueue("Arrow", parent.attackPosition);
            }

            parent.curWeapon.ammo--;
            Debug.Log("CurAmmo : " + parent.curWeapon.ammo);
        }
    }
    float asdf = 0;
    public IEnumerator Reload(TravellerController parent)
    {
        asdf = 0;
        if (!isReloading)
        {
            isReloading = true;
            Debug.Log("Reload");           
            for (int i = 0; i < parent.curWeapon.reloadTime / 0.02f; i++)
            {
                yield return new WaitForSeconds(0.02f);
                asdf += 0.02f;
                Debug.Log(asdf);
            }
            parent.curWeapon.ammo = parent.curWeapon.magazine;
            Debug.Log("CurAmmo : " + parent.curWeapon.ammo);
            isReloading = false;
        }   
    }

    public void OnAfterDeserialize()
    {
        isReloading = false;
    }

    public void OnBeforeSerialize() { }
}
