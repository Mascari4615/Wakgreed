using UnityEngine;
using Cinemachine;

public class NPC : InteractiveObject
{
    [SerializeField] private GameObject panel;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    public override void Interaction()
    { 
        panel.SetActive(true);
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
