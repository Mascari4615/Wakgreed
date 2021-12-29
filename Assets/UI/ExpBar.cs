using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private IntVariable exp, level;
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI expTextField, levelTextField;
    private RectTransform rectTransform;
    private readonly float originX = 450;
    private readonly float originY = 15;

    public void SetExpBarAndText()
    {
        bar.fillAmount = (float)exp.RuntimeValue / (300 * level.RuntimeValue);
        expTextField.SetText(Mathf.Floor((float)exp.RuntimeValue / (300 * level.RuntimeValue) * 100) + "%");
        (rectTransform ? rectTransform : rectTransform = GetComponent<RectTransform>()).sizeDelta 
            = new Vector2(Mathf.Clamp(originX + level.RuntimeValue * 10, originX, 1250), originY);
        levelTextField.SetText($"Lv. {level.RuntimeValue}");
    }
}