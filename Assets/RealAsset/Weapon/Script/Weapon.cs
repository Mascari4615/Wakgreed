using System.Collections;
using UnityEngine;

public enum Rarity { COM, RAR, UNC, LEG }
public enum InputType { ONCE, CONTINUE }
public enum AttackType { MELEE, RANGED }

[CreateAssetMenu]
public class Weapon : ScriptableObject, ISerializationCallbackReceiver
{
    public int ID;
    public new string name;
    public string description;
    public Sprite icon;
    public Rarity rarity;
    public GameObject resource;
    public GameObject[] subResources;
    public Buff[] buffs;
    [SerializeField] private Skill baseAttack;
    [System.NonSerialized] private bool canBaseAttack = true;
    [SerializeField] private Skill skillQ;
    [System.NonSerialized] private bool canSkillQ = true;
    [SerializeField] private Skill skillE;
    [System.NonSerialized] private bool canSkillE = true;
    public int minDamage = 5;
    public int maxDamage = 10;
    public InputType inputType;
    public AttackType attackType;
    public float attackIntervalTime;
    public float attackSpeed = 2;
    public float magazine;
    [System.NonSerialized] public float ammo;
    public float reloadTime;
    public bool isReloading = false;
    [System.NonSerialized] public WaitForSeconds waitReload;

    private IEnumerator ChangeWithDelay(bool changeValue, float delay, System.Action<bool> makeResult)
    {
        // 참고 : https://velog.io/@sonohoshi/10.-Unity에서-일정-시간-이후-값을-바꾸는-방법
        yield return new WaitForSeconds(delay);
        makeResult(changeValue);
    }

    public void BaseAttack()
    {
        if (!canBaseAttack) return;

        if (magazine != 0)
        {
            if (isReloading) return;
            else if (ammo == 0) GameManager.Instance.StartCoroutine(_Reload());
        }

        baseAttack.Use(minDamage, maxDamage);
        canBaseAttack = false;
        GameManager.Instance.StartCoroutine(ChangeWithDelay(true, 1f / attackSpeed, value => canBaseAttack = value));
    }

    public void SkillQ() 
    { 
        if (!canSkillQ || skillQ == null) return;

        skillQ?.Use(minDamage, maxDamage);
        canSkillQ = false;
        GameManager.Instance.StartCoroutine(ChangeWithDelay(true, skillQ.coolTime, value => canSkillQ = value));
    }

    public void SkillE() 
    { 
        if (!canSkillE || skillE == null) return;

        skillE?.Use(minDamage, maxDamage);
        canSkillE = false;
        GameManager.Instance.StartCoroutine(ChangeWithDelay(true, skillE.coolTime, value => canSkillE = value));
    }

    public void Reload()
    {
        if (magazine != 0)
        {
            if (!isReloading) GameManager.Instance.StartCoroutine(_Reload());
        } 
    }

    private IEnumerator _Reload()
    {
        isReloading = true;
        yield return waitReload;
        ammo = magazine;
        isReloading = false;
    }

    public void OnAfterDeserialize()
    {
        ammo = magazine;
        waitReload = new WaitForSeconds(reloadTime);
    }

    public void OnBeforeSerialize() { }
}