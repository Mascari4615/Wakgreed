using System;
using UnityEngine;
[CreateAssetMenu(fileName = "MaxHp", menuName = "Variable/MaxHp")]
public class MaxHp : ScriptableObject
{
    [SerializeField] private IntVariable hpCur;
    public int RuntimeValue
    {
        get
        {
            return RuntimeValue;
        }
        set
        {
            int origin = RuntimeValue;
            RuntimeValue = value;
            hpCur.RuntimeValue = (int)Math.Round(hpCur.RuntimeValue * ((float)RuntimeValue / origin), MidpointRounding.AwayFromZero);
        }
    }
}