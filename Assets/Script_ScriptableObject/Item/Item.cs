using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Variable/Item")]
public class Item : SpecialThing
{
    [TextArea] public string simpleDescription;
    public int price;
}