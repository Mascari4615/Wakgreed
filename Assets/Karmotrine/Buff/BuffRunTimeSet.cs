using UnityEngine;

[CreateAssetMenu(fileName = "BuffRunTimeSet", menuName = "GameSystem/RunTimeSet/Buff", order = 2)]
public class BuffRunTimeSet : RunTimeSet<Buff>
{
    [SerializeField] private GameEvent OnBuffEquip;
    [SerializeField] private GameEvent OnBuffRemove;

    public override void Add(Buff buff)
    {
        base.Add(buff);
        buff.OnEquip();
        OnBuffEquip.Raise();
    }

    public override void Remove(Buff buff)
    {
        base.Remove(buff);
        buff.OnRemove();
        OnBuffRemove.Raise();
    }
}