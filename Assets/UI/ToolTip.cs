using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI weaponDamageText;

    public void SetToolTip(Sprite sprite, string header, string description)
    {
        image.sprite = sprite;
        headerField.text = header;
        descriptionField.text = description;
        weaponDamageText.text = "";
    }

    public void SetToolTip(SpecialThing specialThing)
    {
        image.sprite = specialThing.sprite;
        headerField.text = specialThing.name;
        descriptionField.text = specialThing.description;
        weaponDamageText.text = specialThing is Weapon ? $"{(specialThing as Weapon).minDamage} ~ {(specialThing as Weapon).maxDamage} µ¥¹ÌÁö" : "";
    }
}