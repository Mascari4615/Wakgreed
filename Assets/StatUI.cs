using UnityEngine;
using TMPro;
using System;

public class StatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI evasionText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI staticDefText;
    [SerializeField] private TextMeshProUGUI ciriticalText;
    [SerializeField] private TextMeshProUGUI ciriticalDamageText;
    [SerializeField] private TextMeshProUGUI bossDamage;
    [SerializeField] private TextMeshProUGUI mobDamage;
    [SerializeField] private TotalPower totalPower;
    [SerializeField] private IntVariable ciritical;
    [SerializeField] private IntVariable ciriticalDamage;
    [SerializeField] private IntVariable defence;
    [SerializeField] private IntVariable staticDefence;
    [SerializeField] private IntVariable movesSpeedBonus;
    [SerializeField] private IntVariable miss;
    [SerializeField] private FloatVariable evasion;
    [SerializeField] private TextMeshProUGUI bonusHpText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI movesSpeedBonusText;
    [SerializeField] private TextMeshProUGUI dashChargeSpeedText;
    [SerializeField] private TextMeshProUGUI missText;
    [SerializeField] private MaxHp maxHp;
    [SerializeField] private FloatVariable attackSpeed;
    [SerializeField] private FloatVariable dashChargeSpeed;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        bonusHpText.text = (maxHp.RuntimeValue - Wakgood.Instance.wakdu.baseHp).ToString();
        powerText.text = totalPower.RuntimeValue.ToString();
        defText.text = $"{MathF.Round((1 - 100 / (float)(defence.RuntimeValue + 100)), 2) * 100}%";
        staticDefText.text = staticDefence.RuntimeValue.ToString();

        evasionText.text = $"{MathF.Round((1 - 100 / (float)(evasion.RuntimeValue + 100)), 2) * 100}%";
        ciriticalText.text = ciritical.RuntimeValue.ToString();
        ciriticalDamageText.text= ciriticalDamage.RuntimeValue.ToString();
        bossDamage.text = Wakgood.Instance.BossDamage.RuntimeValue.ToString();
        mobDamage.text = Wakgood.Instance.MobDamage.RuntimeValue.ToString();
        attackSpeedText.text = attackSpeed.RuntimeValue.ToString();
        movesSpeedBonusText.text = movesSpeedBonus.RuntimeValue.ToString();
        missText.text = miss.RuntimeValue.ToString();
        dashChargeSpeedText.text = dashChargeSpeed.RuntimeValue.ToString();
    }
}
