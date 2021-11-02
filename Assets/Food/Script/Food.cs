using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Variable/Food")]
public class Food : Equiptable
{
    [TextArea] public string simpleDescription;
    public int price;
}