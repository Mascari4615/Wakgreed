using System;
using UnityEngine;
[CreateAssetMenu(fileName = "TotalPower", menuName = "Variable/TotalPower")]
public class TotalPower : ScriptableObject
{
    [SerializeField] private IntVariable power;
    [SerializeField] private IntVariable powerPer;
    public int RuntimeValue => (int) Math.Round((power.RuntimeValue) * 
        (1 + powerPer.RuntimeValue * 0.01f), MidpointRounding.AwayFromZero);
}