using UnityEngine;

public class ItemGameObject : InteractiveObject
{
    private Item item;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int itemID)
    {
        item = DataManager.Instance.ItemDic[itemID];
        spriteRenderer.sprite = item.sprite;
    }

    public override void Interaction()
    {
        if (DataManager.Instance.CurGameData.equipedOnceItem[item.id] == false)
        {
            if (Collection.Instance != null)
            {
                Collection.Instance.Collect(item);
            }

            DataManager.Instance.CurGameData.equipedOnceItem[item.id] = true;
            DataManager.Instance.SaveGameData();
        }

        DataManager.Instance.wgItemInven.Add(item);

        gameObject.SetActive(false);
    }
}