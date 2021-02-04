using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractiveObjectType
{ 
    None,  
    Portal,
    TravellerChanger
}

public class InteractionManager : MonoBehaviour
{
    private static InteractionManager instance;
    [HideInInspector] public static InteractionManager Instance { get { return instance; } }

    public GameObject button;
    public GameObject interactionIcon = null;
    public GameObject attackIcon = null;
    [HideInInspector] public InteractiveObjectType nearInteractionObject = InteractiveObjectType.None;

    // TravellerChanger
    [SerializeField] private GameObject travellerChangePanel;

    private void Awake()
    {
        instance = this;
    }

    public void Interaction()
    {
        interactionIcon.SetActive(false);

        if (nearInteractionObject == InteractiveObjectType.Portal)
        {
            StartCoroutine(GameManager.Instance.EnterPortal());
        }
        else if (nearInteractionObject == InteractiveObjectType.TravellerChanger)
        {
            travellerChangePanel.SetActive(true);
        }
    }
}
