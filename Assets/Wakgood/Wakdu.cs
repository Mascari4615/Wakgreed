using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Wakdu : ScriptableObject
{
    public new string name;
    [FormerlySerializedAs("baseHP")] public int baseHp;
    [FormerlySerializedAs("growthHP")] public int growthHp;
    public int baseAD, growthAD;
    [FormerlySerializedAs("baseAS")] public float baseAs;
    [FormerlySerializedAs("growthAS")] public float growthAs;
    public int baseCriticalChance;
    public float baseMoveSpeed;
}
