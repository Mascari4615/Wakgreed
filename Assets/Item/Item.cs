using UnityEngine;

public enum ItemGrade
{
    Common, Uncommon, Legendary
}

[CreateAssetMenu(fileName = "Item", menuName = "Variable/Item")]
public class Item : Sellable
{
    public ItemGrade grade;
}