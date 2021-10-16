using UnityEngine;

public abstract class Skill : SpecialThing
{
    public GameObject resource;
    public GameObject[] subResource;
    public float coolTime;
    public abstract void Use(int minDamage, int maxDamage);
}
