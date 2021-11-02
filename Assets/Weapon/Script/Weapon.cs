using System.Collections;
using UnityEngine;

public enum InputType { Once, Continue }
public enum AttackType { Melee, Ranged }

[CreateAssetMenu]
public class Weapon : Equiptable, ISerializationCallbackReceiver
{
    public ItemGrade grade;
    public GameObject resource;
    public GameObject[] subResources;
    [SerializeField] private Skill baseAttack;
    public Skill skillQ;
    public Skill skillE;
    public int minDamage = 5;
    public int maxDamage = 10;
    public InputType inputType;
    public AttackType attackType;
    public float attackIntervalTime;
    public float attackSpeed = 2;
    public int magazine;
    [System.NonSerialized] public int Ammo;
    public float reloadTime;
    public float CurReloadTime {get; private set;}
    public bool IsReloading {get; private set;}
    private bool canUseSkillE = true;
    public float CurSkillECoolTime { get; private set; }
    private bool canUseSkillQ = true;
    public float CurSkillQCoolTime { get; private set; }
    private bool canUseBaseAttack = true;
    private float curBaseAttackCoolTime;
    private IEnumerator reload;

    public void BaseAttack()
    {
        if (!canUseBaseAttack || IsReloading) return;

        if (magazine != 0 && Ammo == 0)
        {
            Reload();
            return;
        }

        canUseBaseAttack = false;
        GameManager.Instance.StartCoroutine(BaseAttackCoolTime());
        baseAttack.Use();

        if (magazine == 0 || Ammo != 0)
        {
            return;
        }

        Reload();
        return;
    }

    public void Reload()
    {
        if (Ammo == magazine || IsReloading)
        {
            return;
        }

        IsReloading = true;
        GameManager.Instance.StartCoroutine(reload = Reloadd());
    }
    
    private IEnumerator Reloadd()
    {
        CurReloadTime = 0;
        while ((CurReloadTime += Time.deltaTime) < reloadTime) yield return null;
        Ammo = magazine;
        IsReloading = false;
    }

    public void SkillQ() 
    { 
        if (!canUseSkillQ || !skillQ) return;
        canUseSkillQ = false;
        GameManager.Instance.StartCoroutine(SkillQCoolTime());
        skillQ?.Use();
    }

    public void SkillE() 
    { 
        if (!canUseSkillE || !skillE) return;
        canUseSkillE = false;
        GameManager.Instance.StartCoroutine(SkillECoolTime());
        skillE?.Use();
    }
    
    private IEnumerator BaseAttackCoolTime()
    {
        curBaseAttackCoolTime = 1 / attackSpeed;
        do yield return null;
        while ((curBaseAttackCoolTime -= Time.deltaTime) > 0);
        canUseBaseAttack = true;
    }

     private IEnumerator SkillQCoolTime()
    {
        CurSkillQCoolTime = skillQ.coolTime;
        do yield return null;
        while ((CurSkillQCoolTime -= Time.deltaTime) > 0);
        canUseSkillQ = true;
    }

    private IEnumerator SkillECoolTime()
    {
        CurSkillECoolTime = skillE.coolTime;
        do yield return null;
        while ((CurSkillECoolTime -= Time.deltaTime) > 0);
        canUseSkillE = true;
    }

    public override void OnRemove()
    {
        base.OnRemove();
        if (reload is not null) GameManager.Instance.StopCoroutine(reload);
        IsReloading = false;
        CurReloadTime = 0;
    }

    public void OnAfterDeserialize()
    {
        Ammo = magazine;
        IsReloading = false;
    }

    public void OnBeforeSerialize() { }
}