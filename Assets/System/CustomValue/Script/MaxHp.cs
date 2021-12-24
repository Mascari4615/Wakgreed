using System;
using UnityEngine;
[CreateAssetMenu(fileName = "MaxHp", menuName = "Variable/MaxHp")]
public class MaxHp : ScriptableObject
{
    [SerializeField] private IntVariable hpCur;
    [SerializeField] private GameEvent gameEvent;

    public int RuntimeValue
    {
        get
        {
            return runtimeValue;
        }
        set
        {
            int origin = runtimeValue;
            runtimeValue = value;
            hpCur.RuntimeValue = (int)Math.Round(hpCur.RuntimeValue * ((float)runtimeValue / origin), MidpointRounding.AwayFromZero);
            gameEvent.Raise();
        }
    }

    private int runtimeValue;
}