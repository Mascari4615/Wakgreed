using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    [SerializeField] private ToolTipTrigger toolTipTrigger;
    [SerializeField] private Image image;
    public SpecialThing specialThing;
    [SerializeField] private TextMeshProUGUI countTextField;

    public virtual void SetSlot(SpecialThing _specialThing)
    {
        specialThing = _specialThing;
        image.sprite = specialThing.sprite;

        toolTipTrigger.SetToolTip(specialThing);
        toolTipTrigger.enabled = true;

        if (countTextField != null) countTextField.text = DataManager.Instance.WakgoodItemInventory.itemCountDic[(specialThing as Item).ID].ToString();
    }
}