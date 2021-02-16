using UnityEngine;

public class NPC : InteractiveObject
{
    [SerializeField] private GameObject panel;
    
    public override void Interaction()
    { 
        panel.SetActive(true);
    }
}
