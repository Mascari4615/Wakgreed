using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI weaponDamageText;
    [SerializeField] private TextMeshProUGUI gradeText;

    public void SetToolTip(Sprite sprite, string header, string description)
    {
        image.sprite = sprite;
        headerField.text = header;
        headerField.color = Color.white;
        descriptionField.text = description;
        weaponDamageText.text = "";
    }

    public void SetToolTip(SpecialThing specialThing)
    {
        image.sprite = specialThing.sprite;
        headerField.text = specialThing.name;
        if (specialThing is HasGrade)
        {
            headerField.color = DataManager.Instance.GetGradeColor((specialThing as HasGrade).등급);
        }
        else
        {
            headerField.color = Color.white;
        }
        descriptionField.text = specialThing.description;
        weaponDamageText.text = specialThing is Weapon ? $"{(specialThing as Weapon).minDamage} ~ {(specialThing as Weapon).maxDamage} 데미지" : "";
        gradeText.text = specialThing is HasGrade ? $"{(specialThing as HasGrade).등급} 아이템" : "";
    }
}