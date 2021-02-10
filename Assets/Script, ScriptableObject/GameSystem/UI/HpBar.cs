using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private IntVariable travellerMaxHP;
    [SerializeField] private IntVariable travellerHP;
    [SerializeField] private Image image;
    [SerializeField] private Text textField;

    public void SetHpBar()
    {
        image.fillAmount = (float)travellerHP.RuntimeValue / travellerMaxHP.RuntimeValue;
        textField.text = $"{travellerHP.RuntimeValue} / {travellerMaxHP.RuntimeValue}";
    }
}
