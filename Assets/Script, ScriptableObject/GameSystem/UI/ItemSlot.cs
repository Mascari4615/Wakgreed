using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public ToolTipTrigger toolTipTrigger;
    public void SetItemSlot(Item item)
    {
        transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = item.sprite;
        transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().enabled = true;
        transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = $"{item.count + 1} 개";
        toolTipTrigger.SetText(item.name, item.description, item.comment);
        toolTipTrigger.enabled = true;
    }
}