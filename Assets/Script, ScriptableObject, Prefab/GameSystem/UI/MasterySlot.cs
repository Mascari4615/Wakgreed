using UnityEngine;

public class MasterySlot : MonoBehaviour
{
    public ToolTipTrigger toolTipTrigger;
    public void SetMasterySlot(Mastery mastery)
    {
        transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = mastery.sprite;
        transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().enabled = true;
        toolTipTrigger.SetToolTip(mastery.sprite, mastery.name, mastery.description, mastery.comment);
        toolTipTrigger.enabled = true;
    }
}