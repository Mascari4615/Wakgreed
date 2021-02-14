using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ToolTipTrigger toolTipTrigger;
    [SerializeField] private Image image;
    [SerializeField] private Text countTextField;
    [SerializeField] private Text priceTextField;

    public void SetItemSlot(Item item)
    {
        image.sprite = item.sprite;
        image.enabled = true;
        countTextField.text = $"{item.count + 1} 개";
        if (priceTextField is null == false) priceTextField.text = $"{item.price} 냥";
        toolTipTrigger.SetToolTip(item.sprite, item.name, item.description, item.comment);
        toolTipTrigger.enabled = true;
    }
}