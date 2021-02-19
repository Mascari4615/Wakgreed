using UnityEngine;

public abstract class SpecialThing : ScriptableObject
{
    public int ID;
    public new string name;
    [TextArea] public string description;
    [TextArea] public string comment;
    public Sprite sprite;
    [SerializeField] private Effect[] effects;

    public void OnEquip()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i]._Effect();
        }
    }
    
    public void OnRemove()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Return();
        }
    }
}