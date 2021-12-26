using UnityEngine;
using UnityEngine.UI;
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

    public void SetToolTip(Sprite _sprite, string _name, string _description)
    {
        sprite = _sprite;
        header = _name;
        description = _description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sprite == null) return;

        ToolTipManager.Instance.Show(sprite, header, description);
        isShowingThis = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (sprite == null) return;

        ToolTipManager.Instance.Hide();
        isShowingThis = false;
    }

    private void OnDisable()
    {
        if (isShowingThis) ToolTipManager.Instance.Hide();
    }
}