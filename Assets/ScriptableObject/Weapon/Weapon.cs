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
    public Skill baseAttack;
    public Skill skillQ;
    public Skill skillE;
    public int minDamage;
    public int maxDamage;
    public InputType inputType;
    public AttackType attackType;
    public float attackIntervalTime;
    public float attackSpeed;
    public float magazine;
    [System.NonSerialized] public float ammo;
    public float reloadTime;
    public AudioClip soundEffect;
    public AudioClip[] subAudioEffects;

    public void OnAfterDeserialize()
    {
        ammo = magazine;
    }

    public void OnBeforeSerialize() { }
}
