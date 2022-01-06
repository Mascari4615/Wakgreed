using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private MaxHp hpMax;
    [SerializeField] private IntVariable hpCur;
    [SerializeField] private Image red;
    [SerializeField] private Image yellow;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Wakdu wakdu;

    private readonly float originX = 450;
    private readonly float originY = 40;

    public void SetHpBar()
    {
        Debug.Log($"asd");
        textField.SetText($"{hpCur.RuntimeValue}<size=25>/{hpMax.RuntimeValue}");
    }

    private void Update()
    {
        red.fillAmount = Mathf.Lerp(red.fillAmount, (float)hpCur.RuntimeValue / hpMax.RuntimeValue, Time.deltaTime * 5f);
        yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 3f);
    }

    public void SetHpBarWidth() =>
        (rectTransform ? rectTransform : rectTransform = GetComponent<RectTransform>()).sizeDelta
        = new Vector2(Mathf.Clamp(originX + (hpMax.RuntimeValue - wakdu.baseHp) * 3, originX, 1250), originY);
}
