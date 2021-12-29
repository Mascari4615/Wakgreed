using UnityEngine;
using TMPro;

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
    [SerializeField] private IntVariable power;
    [SerializeField] private IntVariable powerPer;
    [SerializeField] private IntVariable ciritical;
    [SerializeField] private IntVariable ciriticalDamage;
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
        defText.text = defence.RuntimeValue.ToString();
        staticDefText.text = staticDefence.RuntimeValue.ToString();
        ciriticalText.text = ciritical.RuntimeValue.ToString();
        ciriticalDamageText.text= ciriticalDamage.RuntimeValue.ToString();
        bossDamage.text = Wakgood.Instance.BossDamage.RuntimeValue.ToString();
        mobDamage.text = Wakgood.Instance.MobDamage.RuntimeValue.ToString();
    }
}
