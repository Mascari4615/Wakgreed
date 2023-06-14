using UnityEngine;

[CreateAssetMenu(fileName = "EffectRoomBuffTest", menuName = "Effect/RoomBuffTest")]
public class EffectRoomBuffTest : Effect
{
    public BuffRunTimeSet BuffRunTimeSet;
    public GameEvent OnFightStart;
    public GameEvent OnFightEnd;
    public Buff[] buffs;

    public override void _Effect()
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
        foreach (Buff buff in buffs)
        {
            buff.OnEquip();
            BuffRunTimeSet.Add(buff);
        }
    }

    private void RemoveBuff()
    {
        Debug.Log("RemoveBuff");
        foreach (Buff buff in buffs)
        {
            buff.OnRemove();
            BuffRunTimeSet.Remove(buff);
        }
    }
}