using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Variable/Buff")]
public class Buff : ScriptableObject
{
    public int ID;
    public new string name;
    public string description;
    public Sprite sprite;
    public bool hasCondition;
    [SerializeField] private float duration;
    [System.NonSerialized] public float removeTime;
    [SerializeField] private Effect[] effects;

    public void Effect()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i]._Effect();
        }

        removeTime = Time.time + duration;
    }

    public void OnRemove()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Return();
        }
    }
}
