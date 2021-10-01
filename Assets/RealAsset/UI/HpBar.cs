using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpBar : MonoBehaviour
{
    [SerializeField] private IntVariable travellerMaxHP;
    [SerializeField] private IntVariable travellerHP;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textField;

    private void Awake()
    {
        SetHpBar();
    }

    public void SetHpBar()
    {
        image.fillAmount = (float)travellerHP.RuntimeValue / travellerMaxHP.RuntimeValue;
        textField.text = $"{travellerHP.RuntimeValue}<size=30><color=#C8C8C8> / {travellerMaxHP.RuntimeValue}";
    }
}
