using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : InteractiveObject
{
    [SerializeField] private MeshRenderer interactionIcon = null;

    private void Awake()
    {
        interactiveObjectType = InteractiveObjectType.Portal;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            interactionIcon.enabled = true;
        }  
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            interactionIcon.enabled = false;
        }     
    }

    public override void Interaction()
    {
        StartCoroutine(GameManager.Instance.EnterPortal());
    }
}
