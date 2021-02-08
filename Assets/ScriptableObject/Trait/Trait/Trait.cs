using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "Variable/Trait" , order = 3)]
public class Trait : ScriptableObject
{
    public int ID;
    public new string name;
    [TextArea]
    public string description;
    public string comment;
    public Sprite sprite;
    public TraitEffect[] effects;
    public void OnEquip()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Effect();
        }
    }
}