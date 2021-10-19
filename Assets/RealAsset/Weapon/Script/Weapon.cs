using System.Collections;
using UnityEngine;

public enum Rarity { COM, RAR, UNC, LEG }
public enum InputType { ONCE, CONTINUE }
public enum AttackType { MELEE, RANGED }

[CreateAssetMenu]
public class Weapon : Equiptable, ISerializationCallbackReceiver
{
    public Rarity rarity;
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
    public float magazine;
    [System.NonSerialized] public float ammo;
    public float reloadTime;
     public float curReloadTime {get; private set;}
    public bool isReloading {get; private set;}
    public bool SkillEcanUse { get; private set; } = true;
    public float SkillEcurCoolTime { get; private set; }
     public bool SkillQcanUse { get; private set; } = true;
    public float SkillQcurCoolTime { get; private set; }
     public bool BaseAttackcanUse { get; private set; } = true;
    public float BaseAttackcurCoolTime { get; private set; }
    private IEnumerator reload;

    public void BaseAttack()
    {
        if (!BaseAttackcanUse || isReloading) return;

        if (magazine != 0 && ammo == 0)
        {
            Reload();
            return;
        }

        BaseAttackcanUse = false;
        GameManager.Instance.StartCoroutine(BaseAttackCoolTime());
        baseAttack.Use();

        if (magazine != 0 && ammo == 0)
        {
            Reload();
            return;
        }
    }

    public void Reload()
    {
        if (ammo != magazine && !isReloading)
        {
            isReloading = true;
            GameManager.Instance.StartCoroutine(reload = Reloadd());     
        }
    }
    
    private IEnumerator Reloadd()
    {
        curReloadTime = 0;
        while ((curReloadTime += Time.deltaTime) < reloadTime) yield return null;
        ammo = magazine;
        isReloading = false;
    }

    public void SkillQ() 
    { 
        if (!SkillQcanUse || !skillQ) return;
        SkillQcanUse = false;
        GameManager.Instance.StartCoroutine(SkillQCoolTime());
        skillQ?.Use();
    }

    public void SkillE() 
    { 
        if (!SkillEcanUse || !skillE) return;
        SkillEcanUse = false;
        GameManager.Instance.StartCoroutine(SkillECoolTime());
        skillE?.Use();
    }
    
    private IEnumerator BaseAttackCoolTime()
    {
        BaseAttackcurCoolTime = 1 / attackSpeed;
        do yield return null;
        while ((BaseAttackcurCoolTime -= Time.deltaTime) > 0);
        BaseAttackcanUse = true;
    }

     private IEnumerator SkillQCoolTime()
    {
        SkillQcurCoolTime = skillQ.coolTime;
        do yield return null;
        while ((SkillQcurCoolTime -= Time.deltaTime) > 0);
        SkillQcanUse = true;
    }

    private IEnumerator SkillECoolTime()
    {
        SkillEcurCoolTime = skillE.coolTime;
        do yield return null;
        while ((SkillEcurCoolTime -= Time.deltaTime) > 0);
        SkillEcanUse = true;
    }

    public override void OnRemove()
    {
        base.OnRemove();
        if (reload is not null) GameManager.Instance.StopCoroutine(reload);
        isReloading = false;
        curReloadTime = 0;
    }

    public void OnAfterDeserialize()
    {
        ammo = magazine;
        isReloading = false;
    }

    public void OnBeforeSerialize() { }
}