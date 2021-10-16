using UnityEngine;

public enum ItemGrade
{
    Common, Uncommon, Legendary, Material
}

[CreateAssetMenu(fileName = "Item", menuName = "Variable/Item")]
public class Item : Equiptable
{
    public ItemGrade itemGrade;
    public int price;
}