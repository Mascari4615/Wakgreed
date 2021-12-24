using UnityEngine;

public class Portal : InteractiveObject
{
    [SerializeField] private GameObject interactionIcon;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            interactionIcon.SetActive(true);
        }  
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            interactionIcon.SetActive(false);

        }
    }

    public override void Interaction()
    {
        StartCoroutine(GameManager.Instance.EnterPortal());
    }
}
