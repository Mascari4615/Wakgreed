using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private MeshRenderer interactionIcon = null;
    void OnTriggerEnter2D(Collider2D other)
    {
        interactionIcon.enabled = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        interactionIcon.enabled = false;
    }
}
