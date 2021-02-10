using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerExitHandler
{ 
    [SerializeField] private string header;
    [SerializeField] private string description;
    [SerializeField] private string comment;
    [SerializeField] private PivotType pivotType;
    private bool isInputting = false;
    private float t = 0;
    private Vector2 position;

    private void OnEnable()
    {
        isInputting = false;
        t = 0;
    }

    private void Update()
    {
        if (isInputting)
        {
            t += Time.deltaTime;
            if (t > 0.5f)
            {
                ToolTipManager.Show(header, description, comment, position, pivotType);
            }
        }
    }

    public void SetText(string _header, string _description, string _comment)
    {
        header = _header;
        description = _description;
        comment = _comment;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isInputting = true;
        position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        position = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isInputting = false;
        t = 0;
        ToolTipManager.Hide();
    }

    public void OnPointerExit(PointerEventData eventData)
    {        
        isInputting = false;
        t = 0;
        ToolTipManager.Hide();
    }
}