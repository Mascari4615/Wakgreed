using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    private static ToolTipManager instace;
    public static ToolTipManager Instance { get { return instace; } }

    [SerializeField] private ToolTip toolTip;

    private void Awake()
    {
        instace = this;
    }

    public void Show(Sprite sprite, string header, string description)
    {
        toolTip.gameObject.SetActive(true);
        toolTip.SetToolTip(sprite, header, description);
        toolTip.transform.position = Input.mousePosition;
        if (toolTip.transform.position.x < -710) toolTip.transform.position = new Vector3(-710, toolTip.transform.position.y, 0);
    }

    private void Update()
    {
        if (toolTip.gameObject.activeSelf)
        {
            toolTip.transform.position = Input.mousePosition;
        }
    }

    public void Hide()
    {
        toolTip.gameObject.SetActive(false);
    }
}
