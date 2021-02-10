using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerChanger : InteractiveObject
{
    [SerializeField] private GameObject travellerChangePanel;

    public override void Interaction()
    {
        travellerChangePanel.SetActive(true);
    }
}
