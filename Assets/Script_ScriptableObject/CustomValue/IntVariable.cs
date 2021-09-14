using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "Variable/Int" , order = 0)]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public int InitialValue;
    public GameEvent GameEvent;

	public int RuntimeValue { get { return runtimeValue; } set { runtimeValue = value; if (GameEvent) GameEvent.Raise(); } }
    [System.NonSerialized] private int runtimeValue;

    public void OnAfterDeserialize()
    {
        RuntimeValue = InitialValue;
    }

    public void OnBeforeSerialize() { }
}
