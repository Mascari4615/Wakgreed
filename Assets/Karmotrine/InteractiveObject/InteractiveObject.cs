using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    private GameObject interactionIcon;

    protected virtual void Awake()
    {
        interactionIcon = transform.Find("InteractionIcon").gameObject;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && interactionIcon != null)
        {
            interactionIcon.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }
    }

    public abstract void Interaction();
}