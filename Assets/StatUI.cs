using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI evasionText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI staticDefText;
    [SerializeField] private TotalPower totalPower;
    [SerializeField] private IntVariable power;
    [SerializeField] private IntVariable powerPer;
    [SerializeField] private IntVariable defence;
    [SerializeField] private IntVariable staticDefence;
    [SerializeField] private FloatVariable evasion;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        powerText.text = totalPower.RuntimeValue.ToString();
        evasionText.text = evasion.RuntimeValue.ToString();
        defText.tag = defence.RuntimeValue.ToString();
        staticDefText.tag = staticDefence.RuntimeValue.ToString();
    }
}
