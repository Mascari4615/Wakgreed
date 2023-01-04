using System.Collections;
using UnityEngine;

public enum InputType { Once, Continue }
public enum AttackType { Melee, Ranged }

[CreateAssetMenu]
public class Weapon : HasGrade, ISerializationCallbackReceiver
{
    public GameObject resource;
    // public GameObject[] subResources;
    [SerializeField] private Skill baseAttack;
    public Skill specialAttack;
    public Skill skillQ;
    public Skill skillE;
    public int minDamage = 5;
    public int maxDamage = 10;
    // public InputType inputType;
    public AttackType attackType;
    // public float attackIntervalTime;
    public float attackSpeed = 2;
    public int magazine;
    [System.NonSerialized] public int Ammo;
    public float reloadTime;
    [System.NonSerialized] public float CurReloadTime;
    [System.NonSerialized] private float curBaseAttackCoolTime;
    [System.NonSerialized] public float CurSpecialAttackCoolTime;
    [System.NonSerialized] public float CurSkillECoolTime;
    [System.NonSerialized] public float CurSkillQCoolTime;
    [System.NonSerialized] public bool IsReloading = false;
    [System.NonSerialized] private bool canUseBaseAttack = true;
    [System.NonSerialized] private bool canUseSpecialAttack = true;
    [System.NonSerialized] private bool canUseSkillE = true;
    [System.NonSerialized] private bool canUseSkillQ = true;
    [System.NonSerialized] private IEnumerator reload;

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
        baseAttack.Use(this);

        Wakgood.Instance.useBaseAttack.Raise();

        if (baseAttack.type.Equals(SkillType.Base))
        {
            if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[53]))
            {
                int per = 5 * DataManager.Instance.wgItemInven.itemCountDic[53];
                if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[52]))
                    per += 3 * DataManager.Instance.wgItemInven.itemCountDic[52];
                if (Random.Range(0, 100) < per)
                    ObjectManager.Instance.PopObject("Ball", Wakgood.Instance.transform.position).GetComponent<BulletMove>().SetDirection((Vector3)Wakgood.Instance.worldMousePoint - Wakgood.Instance.transform.position);
            }
        }

        if (magazine == 0 || Ammo != 0)
        {
            return;
        }

        Reload();
        return;
    }

    public void Reload()
    {
        if (Ammo == magazine + Wakgood.Instance.bonusAmmo.RuntimeValue || IsReloading)
        {
            return;
        }

        IsReloading = true;
        GameManager.Instance.StartCoroutine(reload = Reloadd());
    }

    private IEnumerator Reloadd()
    {
        CurReloadTime = 0;
        while ((CurReloadTime += Time.deltaTime) < (reloadTime * (1 - Wakgood.Instance.reloadSpeed.RuntimeValue / 100))) yield return null;
        Ammo = magazine + Wakgood.Instance.bonusAmmo.RuntimeValue;
        IsReloading = false;
    }

    public void SpecialAttack()
    {
        if (!canUseSpecialAttack || !specialAttack) return;
        canUseSpecialAttack = false;
        GameManager.Instance.StartCoroutine(SpecialAttackCoolTime());
        specialAttack?.Use(this);
    }

    public void SkillQ()
    {
        if (!canUseSkillQ || !skillQ) return;
        canUseSkillQ = false;
        GameManager.Instance.StartCoroutine(SkillQCoolTime());
        skillQ?.Use(this);
    }

    public void SkillE()
    {
        if (!canUseSkillE || !skillE) return;
        canUseSkillE = false;
        GameManager.Instance.StartCoroutine(SkillECoolTime());
        skillE?.Use(this);
    }

    private IEnumerator BaseAttackCoolTime()
    {
        curBaseAttackCoolTime = 1 / (attackSpeed * (1 + Wakgood.Instance.attackSpeed.RuntimeValue / 100));
        do yield return null;
        while ((curBaseAttackCoolTime -= Time.deltaTime) > 0);
        canUseBaseAttack = true;
    }

    private IEnumerator SpecialAttackCoolTime()
    {
        CurSpecialAttackCoolTime = specialAttack.coolTime * (1 - Wakgood.Instance.skillCollBonus.RuntimeValue / 100);
        do yield return null;
        while ((CurSpecialAttackCoolTime -= Time.deltaTime) > 0);
        canUseSpecialAttack = true;
    }

    private IEnumerator SkillQCoolTime()
    {
        CurSkillQCoolTime = skillQ.coolTime * (1 - Wakgood.Instance.skillCollBonus.RuntimeValue / 100);
        do yield return null;
        while ((CurSkillQCoolTime -= Time.deltaTime) > 0);
        canUseSkillQ = true;
    }

    private IEnumerator SkillECoolTime()
    {
        CurSkillECoolTime = skillE.coolTime * (1 - Wakgood.Instance.skillCollBonus.RuntimeValue / 100);
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

    public void OnAfterDeserialize() =>
        Initialize();

    public void Initialize()
    {
        Ammo = magazine;
        CurReloadTime = 0;
        curBaseAttackCoolTime = 0;
        CurSpecialAttackCoolTime = 0;
        CurSkillECoolTime = 0;
        CurSkillQCoolTime = 0;
        IsReloading = false;
        canUseBaseAttack = true;
        canUseSpecialAttack = true;
        canUseSkillE = true;
        canUseSkillQ = true;
        if (reload is not null) GameManager.Instance.StopCoroutine(reload);
    }

    public void OnBeforeSerialize() { }
}