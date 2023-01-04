using UnityEngine;

public class Altar : InteractiveObject
{
    [SerializeField] private IntVariable Gold;
    [SerializeField] private int defaultPrice;
    [SerializeField] private int increaseAmountMin;
    [SerializeField] private int increaseAmountMax;
    private int price;
    private int interactionCount;

    private void OnEnable()
    {
        interactionCount = 0;
        price = defaultPrice;
    }

    public override void Interaction()
    {
        if (interactionCount >= 2)
            return;

        if (Gold.RuntimeValue >= price)
        {
            Gold.RuntimeValue -= price;
            price += Random.Range(increaseAmountMin, increaseAmountMax + 1);

            if (Random.Range(0, 100) <= 60)
            {
                interactionCount++;
                int itemID = DataManager.Instance.GetRandomItemID();
                ObjectManager.Instance.PopObject("ItemGameObject", transform).GetComponent<ItemGameObject>().Initialize(itemID);
            }
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", transform).GetComponent<AnimatedText>().SetText("골두가 부족합니다!", Color.yellow);
        }
    }
}
