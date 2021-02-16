using UnityEngine;

public class TravellerChanger : NPC
{
    [SerializeField] private Traveller[] travellers;
    public void ChangeTraveller(int index)
    {
        canvas.SetActive(false);
        TravellerController.Instance.traveller = travellers[index];
        TravellerController.Instance.Initialize();
    }
}
