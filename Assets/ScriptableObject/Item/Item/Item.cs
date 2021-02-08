using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Variable/Item" , order = 2)]
public class Item : ScriptableObject
{
    public int ID;
    public new string name;
    public string description;
    public Sprite sprite;
    [System.NonSerialized] public int count;
    public ItemEffect[] effects;
    public void OnEquip()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Effect();
        }
    }
}
