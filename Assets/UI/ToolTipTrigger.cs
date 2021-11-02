using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    private Sprite sprite;
    private string header;
    private string description;

    private bool isShowingThis = false;

    public void SetToolTip(SpecialThing specialThing)
    {
        sprite = specialThing.sprite;
        header = specialThing.name;
        description = specialThing.description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipManager.Instance.Show(sprite, header, description);
        isShowingThis = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.Instance.Hide();
        isShowingThis = false;
    }

    private void OnDisable()
    {
        if (isShowingThis) ToolTipManager.Instance.Hide();
    }
}