using UnityEngine;

public enum PivotType
{
    Mastery,
    Item
}

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager current;
    public ToolTip toolTip;
    public RectTransform rectTransform;

    private void Awake()
    {
        current = this;
    }

    public static void Show(Sprite sprite, string header, string description, Vector3 position, PivotType pivotType)
    {
        current.toolTip.gameObject.SetActive(true);
        current.toolTip.SetToolTip(sprite, header, description);
        //  current.rectTransform.pivot = (pivotType == PivotType.Mastery) ? new Vector2(0.5f, -0.8f) : new Vector2(1.2f, 0.5f);
        current.toolTip.transform.position = position;
        if (current.toolTip.transform.position.x < -710) current.toolTip.transform.position = new Vector3(-710, current.toolTip.transform.position.y, 0);
    }

    public static void Hide()
    {
        current.toolTip.gameObject.SetActive(false);
    }
}
