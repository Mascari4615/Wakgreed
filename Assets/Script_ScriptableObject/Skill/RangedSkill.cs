using UnityEngine;
using System.Collections;
using FMODUnity;

[CreateAssetMenu(fileName = "RangedSkill", menuName = "Skill/RangedSkill")]
public class RangedSkill : Skill, ISerializationCallbackReceiver
{
    static bool isReloading = false;

    public override void Use()
    {
        if (isReloading) return;
        if (Wakgood.Instance.curWeapon.ammo > 0)
        {
            RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack", Wakgood.Instance.attackPosition.position);

            if (resource != null)
            {
                ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.attackPosition);
            }
            else
            {
                //ObjectManager.Instance.GetQueue(PoolType.KnightSwordSlash, parent.attackPosition);
                ObjectManager.Instance.PopObject("Arrow", Wakgood.Instance.attackPosition);
            }

            Wakgood.Instance.curWeapon.ammo--;
            Debug.Log("CurAmmo : " + Wakgood.Instance.curWeapon.ammo);
        }
    }
    float asdf = 0;
    public IEnumerator Reload(Wakgood parent)
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
