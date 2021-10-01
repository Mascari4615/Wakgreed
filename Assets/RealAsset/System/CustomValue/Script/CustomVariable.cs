using UnityEngine;

public abstract class CustomVariable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    public T InitialValue;
    public T RuntimeValue { get { return runtimeValue; } set { runtimeValue = value; if (GameEvent) GameEvent.Raise(); } }
    [System.NonSerialized] private T runtimeValue;
    [SerializeField] private GameEvent GameEvent;

    public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
    public void OnBeforeSerialize() { }
}