using UnityEngine;
using Cinemachine;

public abstract class NPC : InteractiveObject
{
    [SerializeField] protected GameObject canvas;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    public override void Interaction()
    { 
        canvas.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cinemachineVirtualCamera.Priority = 100;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cinemachineVirtualCamera.Priority = -100;
        }
    }
}
