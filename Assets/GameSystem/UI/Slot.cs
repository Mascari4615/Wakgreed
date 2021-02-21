using UnityEngine;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour
{
    [SerializeField] private ToolTipTrigger toolTipTrigger;
    [SerializeField] private Image image;
    public SpecialThing specialThing { get; private set; }

    public virtual void SetSlot(SpecialThing _specialThing)
    {
        specialThing = _specialThing;
        image.sprite = specialThing.sprite;

        toolTipTrigger.SetToolTip(specialThing);
        toolTipTrigger.enabled = true;
    }
}