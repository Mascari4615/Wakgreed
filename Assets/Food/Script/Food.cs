using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Variable/Food")]
public class Food : Sellable
{
    [TextArea] public string simpleDescription;
}