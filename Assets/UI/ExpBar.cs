using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private IntVariable exp, level;
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI expTextField, levelTextField; 

    private void Awake()
    {
        SetExpBar();
        SetLevelText();
    }

    public void SetExpBar()
    {
        bar.fillAmount = (float)exp.RuntimeValue / (150 * level.RuntimeValue);
        expTextField.SetText((Mathf.Floor((float)exp.RuntimeValue / (150 * level.RuntimeValue) * 100) + "%"));
    }

    public void SetLevelText() => levelTextField.SetText($"Lv. {level.RuntimeValue}");
}
