using UnityEngine;
using System.Collections;

public abstract class Skill : SpecialThing
{
    public GameObject resource;
    public GameObject[] subResource;
    public float coolTime;
    public abstract void Use();
}
