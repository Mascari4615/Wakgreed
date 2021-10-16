using UnityEngine;

public abstract class SpecialThing : ScriptableObject
{
    public int ID;
    public new string name;
    public Sprite sprite;
    [TextArea] public string description;
}

public abstract class Equiptable : SpecialThing
{
    [SerializeField] private Effect[] effects;

    public virtual void OnEquip()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i]._Effect();
        }
    }

    public virtual void OnRemove()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Return();
        }
    }
}