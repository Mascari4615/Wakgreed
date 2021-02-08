using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private IntVariable travellerEXP;
    [SerializeField] private IntVariable travellerLevel;
    [SerializeField] private Image expBarImage;
    [SerializeField] private Text expText;
    [SerializeField] private Text levelText ; 

    public void SetExpBar()
    {
        expBarImage.fillAmount = (float)travellerEXP.RuntimeValue / (100 * (1 + travellerLevel.RuntimeValue));
        expText.text = Mathf.Floor((float)travellerEXP.RuntimeValue / (100 * (1 + travellerLevel.RuntimeValue)) * 100) + "%";
    }

    public void SetLevelText()
    {
        levelText.text = $"Lv. {travellerLevel.RuntimeValue}";
    }
}
