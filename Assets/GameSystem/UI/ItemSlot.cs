using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : Slot
{
    [SerializeField] private Text countTextField;

    public override void SetSlot(SpecialThing specialThing)
    {
        base.SetSlot(specialThing);

        countTextField.text = $"{(specialThing as Item).count + 1} 개";
    }
}