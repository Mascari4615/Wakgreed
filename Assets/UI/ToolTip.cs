using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text headerField;
    [SerializeField] private Text descriptionField;

    public RectTransform rectTransform;

    public void SetToolTip(Sprite sprite, string header, string description)
    {
        image.sprite = sprite;
        headerField.text = header;
        descriptionField.text = description;
    }
}