using UnityEngine;

public class Chest : InteractiveObject
{
    private int itemID;

    [SerializeField] private float commonWeight;
    [SerializeField] private float uncommonWeight;
    [SerializeField] private float legendaryWeight;

    [SerializeField] private Sprite sprite;

    private SpriteRenderer spriteRenderer;
    private ObjectWithDuration objectWithDuration;
    private Animator animator;
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectWithDuration = GetComponent<ObjectWithDuration>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = sprite;
        objectWithDuration.enabled = false;
        boxCollider2D.enabled = true;

        animator.Play("START", 0);

        Probability<ItemGrade> probability = new();
        probability.Add(ItemGrade.Common, commonWeight);
        probability.Add(ItemGrade.Uncommon,uncommonWeight);
        probability.Add(ItemGrade.Legendary, legendaryWeight);

        itemID = DataManager.Instance.GetRandomItemID(probability.Get());
    }

    public override void Interaction()
    {
        // 상자가 열린 이후 다시 함수가 호출되고 트리거시켜도 이미 애니메이션이 끝났기 때문에 실행되지 않음
        // 유용하게 쓸 수 있는 방법일 듯
        animator.SetTrigger("OPEN");
        objectWithDuration.enabled = true;
        boxCollider2D.enabled = false;
    }

    // 상자가 열리는 애니메이션이 실행될 때 애니메이션 이벤트로 호출됨
    private void OpenChest()
    {
        ObjectManager.Instance.PopObject("ItemGameObject", transform.position).GetComponent<ItemGameObject>().Initialize(itemID);
    }
}