using System;
using UnityEngine;

public abstract class CustomVariable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private T InitialValue;
    [SerializeField] private GameEvent GameEvent;
    [NonSerialized] private T runtimeValue;

    public T RuntimeValue
    {
        get => runtimeValue;
        set
        {
            runtimeValue = value;
            if (GameEvent is not null)
            {
                GameEvent.Raise();
            }
        }
    }

    public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
    public void OnBeforeSerialize() { }
}