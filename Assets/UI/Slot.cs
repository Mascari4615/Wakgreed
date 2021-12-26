using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    [SerializeField] private ToolTipTrigger toolTipTrigger;
    [SerializeField] private Image image;
    public SpecialThing specialThing;
    [SerializeField] private TextMeshProUGUI countTextField;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetSlot(SpecialThing _specialThing)
    {
        specialThing = _specialThing;
        image.sprite = specialThing.sprite;

        if (toolTipTrigger != null)
        {
            toolTipTrigger.SetToolTip(specialThing);
            toolTipTrigger.enabled = true;
        }

        if (countTextField != null) countTextField.text = DataManager.Instance.wakgoodItemInventory.itemCountDic[(specialThing as Item).id].ToString();
        if (nameText != null) nameText.text = _specialThing.name;
        if (priceText != null) priceText.text = (_specialThing as Sellable).price.ToString();
        if (descriptionText != null) descriptionText.text = _specialThing.description;
    }

    public void SetSlot(Monster monster)
    {
        Debug.Log(monster.defaultSprite);
        image.sprite = monster.defaultSprite;

        toolTipTrigger.SetToolTip(image.sprite, monster.mobName, monster.description);
        toolTipTrigger.enabled = true;

        if (nameText != null) nameText.text = monster.mobName;
        if (descriptionText != null) descriptionText.text = monster.description;
    }
}