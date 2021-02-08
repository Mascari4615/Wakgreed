using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "Variable/Float" , order = 1)]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public float InitialValue;

	[System.NonSerialized] public float RuntimeValue;

    public void OnAfterDeserialize()
    {
        RuntimeValue = InitialValue;
    }

    public void OnBeforeSerialize() { }
}
