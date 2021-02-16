using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : Slot
{
    [SerializeField] private Text countTextField;
    [SerializeField] private Text priceTextField;

    public override void SetSlot(SpecialThing specialThing)
    {
<<<<<<< HEAD
        base.SetSlot(specialThing);

        countTextField.text = $"{(specialThing as Item).count + 1} 개";
=======
        image.sprite = item.sprite;
        image.enabled = true;
        countTextField.text = $"{item.count + 1} 개";
        if (priceTextField is null == false) priceTextField.text = $"{item.price} 냥";
        toolTipTrigger.SetToolTip(item.sprite, item.name, item.description, item.comment);
        toolTipTrigger.enabled = true;
>>>>>>> 7195055b16ed9a40fd6ac9cc5ddf829d30020a9f
    }
}