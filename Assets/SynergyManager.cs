using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SynergyManager : MonoBehaviour
{
    [SerializeField] private Synergy[] Synergies;

    public void CheckAllSynergy()
    {
        foreach (Synergy synergy in Synergies)
        {
            if (CheckSynergy(synergy))
            {
                if (synergy.isOn == false)
                {
                    synergy.isOn = true;
                    synergy.OnEquip();
                }
            }
            else
            {
                if (synergy.isOn == true)
                {
                    synergy.OnRemove();
                    synergy.isOn = false;
                }
            }
        }
    }

    private bool CheckSynergy(Synergy synergy)
    {
        foreach (Item item in synergy.SynergyItems)
        {
            if (!DataManager.Instance.wgItemInven.Items.Contains(item))
            {
                return false;
            }
        }

        if (synergy.SynergyWeapon != null)
        {
            if (!Wakgood.Instance.Weapon.Contains(synergy.SynergyWeapon))
            {
                return false;
            }
        }

        return true;
    }
}
