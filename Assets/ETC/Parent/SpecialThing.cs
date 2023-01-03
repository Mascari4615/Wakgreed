using UnityEngine;
using UnityEngine.Serialization;

public enum Grade
{
    Common, Uncommon, Rare, Legendary
}

public class SpecialThing : ScriptableObject
{
    [FormerlySerializedAs("ID")] public int id;
    public new string name;
    public Sprite sprite;
    [TextArea] public string description;
    [SerializeField] private Effect[] effects;

    public virtual void OnEquip()
    {
        if (effects is null) return;
        foreach (Effect t in effects)
            t._Effect();
    }

    public virtual void OnRemove()
    {
        if (effects is null) return;
        foreach (Effect t in effects)
            t.Return();
    }
}

public class HasPrice : SpecialThing
{
    public int price;
}

public class HasGrade : HasPrice
{
    public Grade grade;
}