using UnityEngine;

public class ShopRoom : Room
{
    public override void Enter()
    {
        if (IsVisited == false)
        {
            IsVisited = true;
        }
    }
}
