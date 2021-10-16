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
    public Skill skillQ;
    [System.NonSerialized] private bool canSkillQ = true;
    public Skill skillE;
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
    private bool isReloading = false;
    [System.NonSerialized] public WaitForSeconds waitReload;

    public void BaseAttack()
    {
        // 공격 쿨타임 중이거나, 장전 중이면
        if (!canBaseAttack || isReloading)
        {
            return;
        }

        // 원거린데 총알 없으면
        if (magazine != 0 && ammo == 0)
        {
            // 장전
            GameManager.Instance.StartCoroutine(_Reload());
            return;
        }
;
        // 공격
        baseAttack.Use(minDamage, maxDamage);

        if (magazine != 0 && ammo == 0)
        {
            GameManager.Instance.StartCoroutine(_Reload());
            return;
        }

        // 쿨타임
        canBaseAttack = false;
        GameManager.Instance.StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, 1f / attackSpeed, value => canBaseAttack = value));
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

        UIManager.Instance.reloadUI.SetActive(true);

        float now = 0;
        while (now < reloadTime)
        {
            now += Time.deltaTime;
            UIManager.Instance.reloadImage.fillAmount = now / reloadTime;
            yield return null;
        }

        UIManager.Instance.reloadUI.SetActive(false);

        ammo = magazine;
        isReloading = false;
    }

    public void SkillQ() 
    { 
        if (!canSkillQ || skillQ == null) return;

        skillQ?.Use(minDamage, maxDamage);
        canSkillQ = false;
        GameManager.Instance.StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, skillQ.coolTime, value => canSkillQ = value));
    }

    public void SkillE() 
    { 
        if (!canSkillE || skillE == null) return;

        skillE?.Use(minDamage, maxDamage);
        canSkillE = false;
        GameManager.Instance.StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, skillE.coolTime, value => canSkillE = value));
    }

    public void OnAfterDeserialize()
    {
        ammo = magazine;
        isReloading = false;
    }

    public void OnBeforeSerialize() { }
}