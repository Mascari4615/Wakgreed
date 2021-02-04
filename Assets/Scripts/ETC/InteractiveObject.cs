using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractiveObjectType
{ 
    None,  
    Portal,
    TravellerChanger
}

public class InteractiveObject : MonoBehaviour
{
    public InteractiveObjectType interactiveObjectType;

    public virtual void Interaction()
    {

    }
}
