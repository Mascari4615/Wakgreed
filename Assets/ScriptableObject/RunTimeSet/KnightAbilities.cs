using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "KnightAbilities", menuName = "TravellerAbilities/Knight")]
public class KnightAbilities : TravellerAbilities
{
    private CinemachineImpulseSource cinemachineImpulseSource;

    public override void Initialize(TravellerController t)
    {
        cinemachineImpulseSource = t.GetComponent<CinemachineImpulseSource>();
    }

    public override void _Update(TravellerController t)
    {
        if (t.h > 0)
        {
            // t.weaponPosition.transform.rotation = Quaternion.Euler(0, 0, t.joyStick.inputValue.y * 90);
        }
        else if (t.h < 0)
        {
            // t.weaponPosition.transform.rotation = Quaternion.Euler(0, 0, t.joyStick.inputValue.y * -90);
        }
    }

    public override void BasicAttack(TravellerController t)
    {
        if (t.weaponPosition.transform.localScale == new Vector3(1, 1, 1))
        {
            t.weaponPosition.transform.localPosition = new Vector3(0.1f, 0.4f, 0);
            t.weaponPosition.transform.localScale = new Vector3(-1, -1, 1);
        }
        else
        {
            t.weaponPosition.transform.localPosition = new Vector3(0.55f, 0.5f, 0);
            t.weaponPosition.transform.localScale = new Vector3(1, 1, 1);
        }
        //ObjectManager.Instance.GetQueue(PoolType.KnightSwordSlash, t.attackPosition);
        ObjectManager.Instance.GetQueue("SwordSlash", t.attackPosition);
        cinemachineImpulseSource.GenerateImpulse();
    }

    public override void Skill0(TravellerController t)
    {
        
    }

    public override void Skill1(TravellerController t)
    {
        
    }

    public override void Skill2(TravellerController t)
    {
        
    }
}