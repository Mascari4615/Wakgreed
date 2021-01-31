using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class Player : Traveller
{
    private CinemachineImpulseSource cinemachineImpulseSource;
    private int weaponRot = 0;

    protected override void Awake()
    {
        base.Awake();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    protected override void Move()
    {
        base.Move();
        
        if (h > 0)
        {
            weaponPosition.transform.rotation = Quaternion.Euler(0, 0, joyStick.inputValue.y * 90);
        }
        else if (h < 0)
        {
            weaponPosition.transform.rotation = Quaternion.Euler(0, 0, joyStick.inputValue.y * -90);
        }
    }

    protected override void BasicAttack()
    {
        base.BasicAttack();
        
        if (weaponRot == 0)
        {
            weaponRot = 90;
            weaponPosition.transform.localPosition = new Vector3(0, -0.5f, -0.5f);
            weaponPosition.transform.localScale = new Vector3(-1, -1, 1);
        }
        else if (weaponRot == 90)
        {
            weaponRot = 0;
            weaponPosition.transform.localPosition = new Vector3(0.7f, -0.35f, 0.5f);
            weaponPosition.transform.localScale = new Vector3(1, 1, 1);
        }
        ObjectManager.Instance.GetQueue(PoolType.PlayerDefaultAttack, attackPosition);
        cinemachineImpulseSource.GenerateImpulse();
    }

    protected override void Skill0()
    {
        base.Skill0();
    }

    protected override void Skill1()
    {
        base.Skill1();
    }

    protected override void Skill2()
    {
        base.Skill2();
    }
}