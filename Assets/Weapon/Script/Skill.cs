using UnityEngine;
using System.Collections;

public enum SkillType
{
    Skill,
    Base
}

public abstract class Skill : SpecialThing
{
    public SkillType type;
    public GameObject resource;
    public GameObject[] subResource;
    public float coolTime;
    public abstract void Use(Weapon weapon);
}
