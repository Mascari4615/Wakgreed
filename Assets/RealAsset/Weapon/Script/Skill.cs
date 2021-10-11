using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public int ID;
    public new string name;
    public string description;
    public Sprite icon;
    public GameObject resource;
    public GameObject[] subResource;
    public Buff[] buffs;
    public float coolTime;
    public abstract void Use(int minDamage, int maxDamage);
}
