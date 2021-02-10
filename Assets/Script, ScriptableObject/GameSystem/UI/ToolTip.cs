using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Text headerField;
    [SerializeField] private Text descriptionField;
    [SerializeField] private Text commentField;

    public RectTransform rectTransform;

    public void SetText(string header, string description, string comment)
    {
        headerField.text = header;
        descriptionField.text = description;
        commentField.text = comment;
    }
}