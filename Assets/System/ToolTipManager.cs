using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance { get; private set; }

    [SerializeField] private ToolTip toolTip;
    [SerializeField] private RectTransform toolTipBackGround;
    private float toolTipWidth;
    private float toolTipHeight;
    private float toolTipFloatAmount;
    private const float ToolTipPadding = 30f;

    private void Awake()
    {
        Instance = this;
        toolTipWidth = toolTipBackGround.sizeDelta.x;
        toolTipHeight = toolTipBackGround.sizeDelta.y;
        toolTipFloatAmount = toolTipBackGround.localPosition.y;
    }

    public void Show(Sprite sprite, string header, string description)
    {
        toolTip.gameObject.SetActive(true);
        toolTip.SetToolTip(sprite, header, description);
        toolTip.transform.position = new Vector3(Mathf.Clamp(Input.mousePosition.x, toolTipWidth / 2, Screen.width - toolTipWidth / 2), Input.mousePosition.y, 0);
    }

    private void Update()
    {
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.transform.position = new Vector3(
                Mathf.Clamp(Input.mousePosition.x, toolTipWidth / 2 + ToolTipPadding, Screen.width - toolTipWidth / 2 - ToolTipPadding),
                Mathf.Clamp(Input.mousePosition.y, toolTipHeight / 2 + ToolTipPadding - toolTipFloatAmount, Screen.height - toolTipHeight / 2 - ToolTipPadding - toolTipFloatAmount), 0);
        }
    }

    public void Hide()
    {
        toolTip.gameObject.SetActive(false);
    }
}
