using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Wakdu : ScriptableObject
{
    public new string name;
    public int baseHp;
    public int growthHp;
    [FormerlySerializedAs("baseAD")] public int basePower;
    [FormerlySerializedAs("growthAD")] public int growthPower;
    [FormerlySerializedAs("baseAs")] public float baseAttackSpeed;
    [FormerlySerializedAs("growthAs")] public float growthAttackSpeed;
    public float baseMoveSpeed;
}
