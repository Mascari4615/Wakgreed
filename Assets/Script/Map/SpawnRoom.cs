public class SpawnRoom : Room
{
    public override void Enter()
    {
        if (IsVisited == false)
        {
            IsVisited = true;
        }

        DoorHiders[0].SetActive(!IsConnectToNearbyRoom[0]);
        DoorHiders[1].SetActive(!IsConnectToNearbyRoom[1]);
        DoorHiders[2].SetActive(!IsConnectToNearbyRoom[2]);
        DoorHiders[3].SetActive(!IsConnectToNearbyRoom[3]);
    }
}