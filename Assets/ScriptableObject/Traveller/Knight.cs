﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

[CreateAssetMenu]
public class Knight : TravellerFunctions
{
    private CinemachineImpulseSource cinemachineImpulseSource;
    private int weaponRot = 0;

    public override void Initialize(Traveller t)
    {
        cinemachineImpulseSource = t.travellerController.GetComponent<CinemachineImpulseSource>();
    }

    public override void _Update(Traveller t)
    {
        if (t.h > 0)
        {
            t.weaponPosition.transform.rotation = Quaternion.Euler(0, 0, t.joyStick.inputValue.y * 90);
        }
        else if (t.h < 0)
        {
            t.weaponPosition.transform.rotation = Quaternion.Euler(0, 0, t.joyStick.inputValue.y * -90);
        }
    }

    public override void BasicAttack(Traveller t)
    {
        if (weaponRot == 0)
        {
            weaponRot = 90;
            t.weaponPosition.transform.localPosition = new Vector3(0, -0.5f, -0.5f);
            t.weaponPosition.transform.localScale = new Vector3(-1, -1, 1);
        }
        else if (weaponRot == 90)
        {
            weaponRot = 0;
            t.weaponPosition.transform.localPosition = new Vector3(0.7f, -0.35f, 0.5f);
            t.weaponPosition.transform.localScale = new Vector3(1, 1, 1);
        }
        ObjectManager.Instance.GetQueue(PoolType.KnightSwordSlash, t.attackPosition);
        cinemachineImpulseSource.GenerateImpulse();
    }

    public override void Ability0(Traveller t)
    {
        
    }

    public override void Ability1(Traveller t)
    {
        
    }

    public override void Ability2(Traveller t)
    {
        
    }
}