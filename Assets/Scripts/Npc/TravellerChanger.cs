using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerChanger : InteractiveObject
{
    private GameObject currentTravellerGameObject = null;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject bow;

    private void Awake()
    {
        interactiveObjectType = InteractiveObjectType.TravellerChanger;
        currentTravellerGameObject = sword;
    }

    public void ChangeTraveller(int travellerType)
    {
        currentTravellerGameObject.SetActive(false);

        switch (travellerType)
        {
            case 0:
            currentTravellerGameObject = sword;
            break;

            case 1:
            currentTravellerGameObject = bow;
            break;
        }

        currentTravellerGameObject.SetActive(true);
    }
}
