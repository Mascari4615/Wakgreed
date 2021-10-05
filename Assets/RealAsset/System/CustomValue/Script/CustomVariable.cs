using UnityEngine;

public abstract class CustomVariable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private T InitialValue;
    public T RuntimeValue
    {
        get
        {
            return runtimeValue;
        }
        set
        {
            runtimeValue = value;
            if (GameEvent is not null)
                GameEvent.Raise();
        }
    }
    [System.NonSerialized] private T runtimeValue;
    [SerializeField] private GameEvent GameEvent;

    public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
    public void OnBeforeSerialize() { }
}