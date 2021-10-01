using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Variable/Food")]
public class Food : SpecialThing
{
    [TextArea] public string simpleDescription;
    public int price;
}