public class SpawnRoom : Room
{
    public override void Enter()
    {
        if (isVisited == false) isVisited = true;
        doorHiders[0].SetActive(!isConnectToNearbyRoom[0]);
        doorHiders[1].SetActive(!isConnectToNearbyRoom[1]);
        doorHiders[2].SetActive(!isConnectToNearbyRoom[2]);
        doorHiders[3].SetActive(!isConnectToNearbyRoom[3]);
    }
}
