using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    private const float ToolTipPadding = 30f;

    [SerializeField] private ToolTip toolTip;
    private float toolTipHeight;
    private float toolTipWidth;
    public static ToolTipManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        RectTransform rectTransform = toolTip.GetComponent<RectTransform>();
        toolTipWidth = rectTransform.sizeDelta.x;
        toolTipHeight = rectTransform.sizeDelta.y;
    }

    private void Update()
    {
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.transform.position = GetVec();
        }
    }

    public void Show(Sprite sprite, string header, string description)
    {
        toolTip.SetToolTip(sprite, header, description);
        toolTip.transform.position = GetVec();
        toolTip.gameObject.SetActive(true);
    }

    public void Show(SpecialThing specialThing)
    {
        toolTip.SetToolTip(specialThing);
        toolTip.transform.position = GetVec();
        toolTip.gameObject.SetActive(true);
    }

    private Vector3 GetVec()
    {
        return new Vector3(
            Mathf.Clamp(Input.mousePosition.x, (toolTipWidth / 2) + ToolTipPadding,
                Screen.width - (toolTipWidth / 2) - ToolTipPadding),
            Mathf.Clamp(Input.mousePosition.y + 40, ToolTipPadding, Screen.height - toolTipHeight - ToolTipPadding), 0);
    }

    public void Hide()
    {
        toolTip.gameObject.SetActive(false);
    }
}