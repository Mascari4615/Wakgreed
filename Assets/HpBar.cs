using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private IntVariable travellerMaxHP;
    [SerializeField] private IntVariable travellerHP;
    [SerializeField] private Image image;
    [SerializeField] private Text text;

    public void SetHpBar()
    {
        image.fillAmount = (float)travellerHP.RuntimeValue / travellerMaxHP.RuntimeValue;
        text.text = $"{travellerHP.RuntimeValue} / {travellerMaxHP.RuntimeValue}";
    }
}
