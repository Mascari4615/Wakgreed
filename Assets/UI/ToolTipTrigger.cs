using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private bool isSpecialThings = true;
    private SpecialThing specialThing;

    private Sprite sprite;
    private string header;
    private string description;

    private bool isShowingThis = false;

    public void SetToolTip(SpecialThing _specialThing) =>
        specialThing = _specialThing;

    public void SetToolTip(Sprite _sprite, string _name, string _description)
    {
        sprite = _sprite;
        header = _name;
        description = _description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSpecialThings)
            ToolTipManager.Instance.Show(specialThing);
        else
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