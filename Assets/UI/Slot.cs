using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public ToolTipTrigger toolTipTrigger;
    [SerializeField] private Image image;
    public SpecialThing SpecialThing { get; private set; }
    [SerializeField] private TextMeshProUGUI countTextField;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetSlot(SpecialThing _specialThing)
    {
        SpecialThing = _specialThing;
        image.sprite = SpecialThing.sprite;

        if (toolTipTrigger != null)
            toolTipTrigger.SetToolTip(SpecialThing);
   
        if (countTextField != null) countTextField.text = DataManager.Instance.wgItemInven.itemCountDic[(SpecialThing as Item).id].ToString();
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