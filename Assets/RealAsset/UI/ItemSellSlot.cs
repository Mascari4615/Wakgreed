using UnityEngine;
using UnityEngine.UI;

public class ItemSellSlot : Slot
{
    [SerializeField] private Text priceTextField;

    public override void SetSlot(SpecialThing specialThing)
    {
        base.SetSlot(specialThing);

        priceTextField.text = (specialThing as Item).price.ToString();
    }
}
