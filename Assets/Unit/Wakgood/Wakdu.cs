using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Wakdu : ScriptableObject
{
    public new string name;
    public int baseHp;
    public int growthHp;
    public int basePower;
    public int growthPower;
    public float baseAttackSpeed;
    public float growthAttackSpeed;
    public float baseMoveSpeed;
    public RuntimeAnimatorController controller;
    public Sprite defaultSprite;
}
