using UnityEngine;

public class Altar : InteractiveObject
{
    [SerializeField] private IntVariable Gold;

    public override void Interaction()
    {
        if (Gold.RuntimeValue >= 50)
        {
            Gold.RuntimeValue -= 50;
            ObjectManager.Instance.PopObject("Item", transform).GetComponent<ItemGameObject>().InitializeRandom();
        }
        else
        {
            Debug.Log("Not Enough Gold");
        }
    }
}
