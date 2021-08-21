using UnityEngine;
using TMPro;

public class ItemSlot : Slot
{
    [SerializeField] private TextMeshProUGUI countTextField;

    public override void SetSlot(SpecialThing specialThing)
    {
        base.SetSlot(specialThing);

        if (countTextField != null) countTextField.text = DataManager.Instance.ItemInventory.itemCountDic[(specialThing as Item).ID].ToString(); 
    }
}