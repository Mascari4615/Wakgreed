using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    [SerializeField] private ToolTipTrigger toolTipTrigger;
    [SerializeField] private Image image;
    public SpecialThing specialThing;
    [SerializeField] private TextMeshProUGUI countTextField;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetSlot(SpecialThing _specialThing)
    {
        specialThing = _specialThing;
        image.sprite = specialThing.sprite;

        toolTipTrigger.SetToolTip(specialThing);
        toolTipTrigger.enabled = true;

        if (countTextField != null) countTextField.text = DataManager.Instance.wakgoodItemInventory.itemCountDic[(specialThing as Item).id].ToString();
        if (nameText != null) nameText.text = _specialThing.name;
        if (descriptionText != null) descriptionText.text = _specialThing.description;
    }

    public void SetSlot(Monster monster)
    {
        image.sprite = monster.GetComponent<SpriteRenderer>().sprite;
        toolTipTrigger.enabled = false;

        if (nameText != null) nameText.text = monster.name;
        if (descriptionText != null) descriptionText.text = monster.description;
    }
}