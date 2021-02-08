using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffectRoomBuffTest", menuName = "ItemEffect/RoomBuffTest" , order = int.MaxValue)]
public class ItemEffectRoomBuffTest : ItemEffect
{
    public GameEvent OnFightStart;
    public GameEvent OnFightEnd;
    public override void Effect()
    {
        OnFightStart.AddCollback(AddBuff);
        OnFightEnd.AddCollback(RemoveBuff);
    }

    public override void Return()
    {
        OnFightStart.RemoveCollback(AddBuff);
        OnFightEnd.RemoveCollback(RemoveBuff);
    }

    private void AddBuff()
    {
        Debug.Log("GetBuff");
    }

    private void RemoveBuff()
    {
        Debug.Log("RemoveBuff");
    }
}