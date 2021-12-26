using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private MaxHp hpMax;
    [SerializeField] private IntVariable hpCur;
    [SerializeField] private Image red;
    [SerializeField] private Image yellow;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Wakdu wakdu;
    private bool isUpdating = false;

    private readonly float originX = 450;
    private readonly float originY = 40;

    public void SetHpBar()
    {
        textField.SetText($"{hpCur.RuntimeValue}<size=25>/{hpMax.RuntimeValue}");
        if (!isUpdating)
            StartCoroutine(UpdateHpBar());
    }
    
    private IEnumerator UpdateHpBar()
    {
        isUpdating = true;
        yellow.fillAmount = red.fillAmount;

        while (Mathf.Abs(red.fillAmount - yellow.fillAmount) > 0.002f)
        {
            red.fillAmount = Mathf.Lerp(red.fillAmount, (float)hpCur.RuntimeValue / hpMax.RuntimeValue, Time.deltaTime * 15f);
            yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 3f);
            yield return null;
        }

        yellow.fillAmount = red.fillAmount;
        isUpdating = false;
    }

    public void SetHpBarWidth()
    {
        rectTransform.sizeDelta = new Vector2(Mathf.Clamp(originX + (hpMax.RuntimeValue - wakdu.baseHp) * 10, originX, 1250), originY);
    }
}
