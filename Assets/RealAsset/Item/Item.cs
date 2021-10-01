using UnityEngine;

public enum ItemGrade
{
    Common, Uncommon, Legendary, Material
}

[CreateAssetMenu(fileName = "Item", menuName = "Variable/Item")]
public class Item : SpecialThing
{
    [TextArea] public string simpleDescription;
    public ItemGrade itemGrade;
    public int price;
}