using UnityEngine;

public enum ItemGrade
{
    Common, Uncommon, Legendary
}

[CreateAssetMenu(fileName = "Item", menuName = "Variable/Item")]
public class Item : Equiptable
{
    public ItemGrade grade;
    public int price;
}