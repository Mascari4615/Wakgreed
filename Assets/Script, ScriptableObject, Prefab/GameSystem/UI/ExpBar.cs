using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private IntVariable travellerEXP;
    [SerializeField] private IntVariable travellerLevel;
    [SerializeField] private Image expBarImage;
    [SerializeField] private Text expTextField;
    [SerializeField] private Text levelTextField; 

    public void SetExpBar()
    {
        expBarImage.fillAmount = (float)travellerEXP.RuntimeValue / (100 * (1 + travellerLevel.RuntimeValue));
        expTextField.text = Mathf.Floor((float)travellerEXP.RuntimeValue / (100 * (1 + travellerLevel.RuntimeValue)) * 100) + "%";
    }

    public void SetLevelText()
    {
        levelTextField.text = $"Lv. {travellerLevel.RuntimeValue}";
    }
}
