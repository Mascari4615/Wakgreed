using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "Variable/Int" , order = 0)]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public int InitialValue;
	[System.NonSerialized] public int RuntimeValue;

    public void OnAfterDeserialize()
    {
        RuntimeValue = InitialValue;
    }

    public void OnBeforeSerialize() { }
}
