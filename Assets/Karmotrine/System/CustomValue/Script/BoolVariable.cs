using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolVariable", menuName = "Variable/Bool")]
public class BoolVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private bool InitialValue;
    [SerializeField] private GameEvent GameEventOnTrue;
    [SerializeField] private GameEvent GameEventOnFalse;
    [NonSerialized] private bool runtimeValue;

    public bool RuntimeValue
    {
        get => runtimeValue;
        set
        {
            runtimeValue = value;
            if (value is true && GameEventOnTrue is not null)
            {
                GameEventOnTrue.Raise();
            }
            else if (value is false && GameEventOnFalse is not null)
            {
                GameEventOnFalse.Raise();
            }
        }
    }

    public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
    public void OnBeforeSerialize() { }
}