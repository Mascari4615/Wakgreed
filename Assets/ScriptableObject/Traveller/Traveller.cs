using UnityEngine;

[CreateAssetMenu]
public class Traveller : ScriptableObject
{
    public new string name;
    public int baseHP, growthHP;
    public int baseAD, growthAD;
    public float baseAS, growthAS;
    public int baseCriticalChance;
    public float baseMoveSpeed;
}
