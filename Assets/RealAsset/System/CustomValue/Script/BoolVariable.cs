using UnityEngine;
[CreateAssetMenu(fileName = "BoolVariable", menuName = "Variable/Bool")]
public class BoolVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private bool InitialValue;
    public bool RuntimeValue
    {
        get
        {
            return runtimeValue;
        }
        set
        {
            runtimeValue = value;
            if (value is true && GameEventOnTrue is not null)
                GameEventOnTrue.Raise();
            else if (value is false && GameEventOnFalse is not null)
                GameEventOnFalse.Raise();
        }
    }
    [System.NonSerialized] private bool runtimeValue;
    [SerializeField] private GameEvent GameEventOnTrue;
    [SerializeField] private GameEvent GameEventOnFalse;

    public void OnAfterDeserialize() { RuntimeValue = InitialValue; }
    public void OnBeforeSerialize() { }
}