using UnityEngine;

public abstract class Item : SpecialThing
{
    [TextArea] public string simpleDescription;
    public int price;
    [System.NonSerialized] public int count;
}
