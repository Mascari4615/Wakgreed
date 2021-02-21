using UnityEngine;

[CreateAssetMenu(fileName = "MasteryInventory", menuName = "GameSystem/RunTimeSet/MasteryInventory")]
public class MasteryInventory : RunTimeSet<Mastery>
{
    [SerializeField] private GameEvent OnMasteryInventroyChange;

    public override void Add(Mastery t)
    {
        base.Add(t);
        OnMasteryInventroyChange.Raise();
    }

    public override void Remove(Mastery t)
    {
        base.Remove(t);
        OnMasteryInventroyChange.Raise();
    }
}