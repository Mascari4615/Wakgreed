using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MaxHp", menuName = "Variable/MaxHp")]
public class MaxHp : ScriptableObject
{
    [SerializeField] private IntVariable hpCur;
    [SerializeField] private GameEvent gameEvent;

    private int runtimeValue;

    public int RuntimeValue
    {
        get => runtimeValue;
        set
        {
            int origin = runtimeValue;
            runtimeValue = value;
            hpCur.RuntimeValue = (int)Math.Round(hpCur.RuntimeValue * ((float)runtimeValue / origin),
                MidpointRounding.AwayFromZero);
            gameEvent.Raise();
        }
    }
}