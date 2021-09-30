using UnityEngine;

public abstract class CustomVariable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    public T InitialValue;  
    public T RuntimeValue { get { return runtimeValue; } set { runtimeValue = value; Debug.Log(value + " _ " + runtimeValue); if (GameEvent) GameEvent.Raise(); } }
    [System.NonSerialized] private T runtimeValue;
    public GameEvent GameEvent;

    public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
    public void OnBeforeSerialize() { }
}