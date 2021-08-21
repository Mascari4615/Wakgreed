using UnityEngine;

[CreateAssetMenu]
public abstract class Skill : ScriptableObject
{
    public int ID;
    public new string name;
    public string description;
    public Sprite icon;
    public GameObject resource;
    public GameObject[] subResource;
    public Buff[] buffs;
    public int minDamage;
    public int maxDamage;
    public abstract void Use();
}
