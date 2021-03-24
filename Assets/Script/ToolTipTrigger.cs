using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{ 
    private Sprite sprite;
    private string header;
    private string description;
    private string comment;
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
            if (t > 0.25f)
            {
                ToolTipManager.Show(sprite, header, description, comment, position, pivotType);
            }
        }
    }

    public void SetToolTip(SpecialThing specialThing)
    {
        sprite = specialThing.sprite;
        header = specialThing.name;
        description = specialThing.description;
        comment = specialThing.comment;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInputting = true;
        position = eventData.position;
    }

    public void OnPointerExit(PointerEventData eventData)
    {        
        isInputting = false;
        t = 0;
        ToolTipManager.Hide();
    }
}