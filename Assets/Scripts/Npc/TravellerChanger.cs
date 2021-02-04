using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerChanger : InteractiveObject
{
    [SerializeField] private GameObject travellerChangePanel;
    private GameObject currentTravellerGameObject;
    [SerializeField] private GameObject travellerGameObjectsParent;
    private GameObject[] travellerGameObjects;

    private void Awake()
    {
        interactiveObjectType = InteractiveObjectType.TravellerChanger;
        currentTravellerGameObject = Traveller.Instance.gameObject;
        travellerGameObjects = new GameObject[travellerGameObjectsParent.transform.childCount];

        for (int i = 0; i < travellerGameObjectsParent.transform.childCount; i++)
        {
            travellerGameObjects.SetValue(travellerGameObjectsParent.transform.GetChild(i).gameObject, i);
        }
    }

    public void ChangeTraveller(int travellerIndex)
    {
        currentTravellerGameObject.SetActive(false);
        currentTravellerGameObject = travellerGameObjects[travellerIndex];
        currentTravellerGameObject.SetActive(true);
    }

    public override void Interaction()
    {
        travellerChangePanel.SetActive(true);
    }
}
