using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : Slot
{
    [SerializeField] private TextMeshProUGUI countTextField;

    public override void SetSlot(SpecialThing specialThing)
    {
        base.SetSlot(specialThing);

        if (countTextField != null) countTextField.text = $"{(specialThing as Item).count + 1}";
    }
}