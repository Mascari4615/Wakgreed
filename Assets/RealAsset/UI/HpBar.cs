using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private IntVariable travellerMaxHP;
    [SerializeField] private IntVariable travellerHP;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textField;

    private float originX = 400;
    private float originY = 25;

    public void SetHpBar()
    {
        image.fillAmount = (float)travellerHP.RuntimeValue / travellerMaxHP.RuntimeValue;
        textField.text = $"{travellerHP.RuntimeValue}<size=30><color=#C8C8C8> / {travellerMaxHP.RuntimeValue}";
    }

    public void SetHpBarWidth()
    {
        rectTransform.sizeDelta = new Vector2(originX + (travellerMaxHP.RuntimeValue - Wakgood.Instance.wakdu.baseHP) * 3, originY);
    }
}
