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

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        
        if (other.tag == "Player")
        {
            interactionIcon.enabled = true;
        }  
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        if (other.tag == "Player")
        {
            interactionIcon.enabled = false;
        }     
    }
}
