using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private IntVariable travellerMaxHp;
    [SerializeField] private IntVariable travellerHp;
    [SerializeField] private Image red;
    [SerializeField] private Image yellow;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Wakdu wakdu;
    private bool isUpdating = false;

    private float originX;
    private float originY;

    private void Awake()
    {
        originX = rectTransform.sizeDelta.x;
        originY = rectTransform.sizeDelta.y;
    }

    public void SetHpBar()
    {
        textField.SetText($"{travellerHp.RuntimeValue}<size=25>/{travellerMaxHp.RuntimeValue}");
        if (!isUpdating) StartCoroutine(UpdateHpBar());
    }
    
    private IEnumerator UpdateHpBar()
    {
        isUpdating = true;
        float ratio;

        while ((Mathf.Abs((ratio = (float)travellerHp.RuntimeValue / travellerMaxHp.RuntimeValue) - red.fillAmount)) > 0.002f || Mathf.Abs(red.fillAmount - yellow.fillAmount) > 0.002f)
        {
            red.fillAmount = Mathf.Lerp(red.fillAmount, ratio, Time.deltaTime * 15f);
            yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 5f);
  
            if (Mathf.Abs(red.fillAmount - yellow.fillAmount) < 0.002f)
                yellow.fillAmount = red.fillAmount;

            yield return null;
        }
        
        isUpdating = false;
    }

    public void SetHpBarWidth()
    {
        rectTransform.sizeDelta = new Vector2( Mathf.Clamp(originX + (travellerMaxHp.RuntimeValue - wakdu.baseHp) * 3, originX, 1250), originY);
    }
}
