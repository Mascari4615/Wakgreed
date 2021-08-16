using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Variable/Buff")]
public class Buff : SpecialThing
{
    public bool hasCondition;
    [SerializeField] private float duration;
    [System.NonSerialized] public float removeTime;

    public override void OnEquip()
    {
        base.OnEquip();

        removeTime = Time.time + duration;
        // Debug.Log($"{name} : Effect, RemoveTime - {removeTime}");
    }
}
