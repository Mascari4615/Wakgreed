public class SpawnRoom : Room
{
    public override void Enter()
    {
        if (IsVisited == false)
        {
            IsVisited = true;
        }
    }
}