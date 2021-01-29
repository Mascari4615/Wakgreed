using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class Player : Traveller
{
    private int weaponRot = 0;

    public override void Move()
    {
        base.Move();
        
        if (h > 0)
        {
            weaponPos.transform.rotation = Quaternion.Euler(0, 0, joyStick.inputValue.y * 90);
        }
        else if (h < 0)
        {
            weaponPos.transform.rotation = Quaternion.Euler(0, 0, joyStick.inputValue.y * -90);
        }
    }

    public override void Attack()
    {
        if (weaponRot == 0)
        {
            weaponRot = 90;
            weaponPos.transform.localPosition = new Vector3(0, -0.5f, -0.5f);
            weaponPos.transform.localScale = new Vector3(-1, -1, 1);
        }
        else if (weaponRot == 90)
        {
            weaponRot = 0;
            weaponPos.transform.localPosition = new Vector3(0.7f, -0.35f, 0.5f);
            weaponPos.transform.localScale = new Vector3(1, 1, 1);
        }
        ObjectManager.Instance.GetQueue(PoolType.PlayerDefaultAttack, attackPos);
        audioSource.clip = audioClips[Random.Range(0,audioClips.Length)];
        audioSource.Play();
        cinemachineImpulseSource.GenerateImpulse();
    }
}