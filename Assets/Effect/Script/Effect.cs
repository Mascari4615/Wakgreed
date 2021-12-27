using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [TextArea] public string description;
    public abstract void _Effect();
    public abstract void Return();
}
