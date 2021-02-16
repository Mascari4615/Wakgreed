using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Variable/Item" , order = 2)]
public class Item : ScriptableObject
{
    public int ID;
    public new string name;
    [TextArea] public string description;
    [TextArea] public string simpleDescription;
    [TextArea] public string comment;
    public Sprite sprite;
    public int price;
    [System.NonSerialized] public int count;
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
