using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text headerField;
    [SerializeField] private Text descriptionField;
    [SerializeField] private Text commentField;

    public RectTransform rectTransform;

    public void SetToolTip(Sprite sprite, string header, string description, string comment)
    {
        image.sprite = sprite;
        headerField.text = header;
        descriptionField.text = description;
        commentField.text = comment;
    }
}