using UnityEngine;

public abstract class Item : SpecialThing
{
    [TextArea] public string simpleDescription;
<<<<<<< HEAD
=======
    [TextArea] public string comment;
    public Sprite sprite;
>>>>>>> 7195055b16ed9a40fd6ac9cc5ddf829d30020a9f
    public int price;
    [System.NonSerialized] public int count;
}
