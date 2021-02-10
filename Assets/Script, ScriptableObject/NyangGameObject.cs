using UnityEngine;

public class NyangGameObject : Loot
{
    [SerializeField] private IntVariable Nyang;
    protected override void OnEquip()
    {
        Nyang.RuntimeValue += Random.Range(8, 17); 
    }
}
