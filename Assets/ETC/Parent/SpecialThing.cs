using UnityEngine;
using UnityEngine.Serialization;

public abstract class SpecialThing : ScriptableObject
{
    [FormerlySerializedAs("ID")] public int id;
    public new string name;
    public Sprite sprite;
    [TextArea] public string description;
}

public class Sellable : Equiptable
{
    public int price;
}

public abstract class Equiptable : SpecialThing
{
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