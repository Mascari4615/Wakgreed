using UnityEngine;
using FMODUnity;

public class Chest : InteractiveObject
{
    private bool isOpened;
    private bool isItem;
    private int itemID;

    [SerializeField] private float commonWeight;
    [SerializeField] private float uncommonWeight;
    [SerializeField] private float legendaryWeight;

    [SerializeField] private Sprite sprite;

    private SpriteRenderer spriteRenderer;
    private ObjectWithDuration objectWithDuration;
    private Animator animator;
    private new Collider2D collider2D;
    
    private static readonly int open = Animator.StringToHash("OPEN");

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();
        objectWithDuration = GetComponent<ObjectWithDuration>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        // spriteRenderer.sprite = sprite;
        objectWithDuration.enabled = false;
        collider2D.enabled = true;

        animator.Play("START", 0);

        Probability<ItemGrade> probability = new();
        probability.Add(ItemGrade.Common, commonWeight);
        probability.Add(ItemGrade.Uncommon,uncommonWeight);
        probability.Add(ItemGrade.Legendary, legendaryWeight);

        isOpened = false;
        isItem = Random.Range(0, 100) > 30;
        itemID = isItem ? DataManager.Instance.GetRandomItemID(probability.Get()) : DataManager.Instance.GetRandomWeaponID(probability.Get());
    }

    public override void Interaction()
    {
        if (isOpened) return;
        isOpened = true;

        // 상자가 열린 이후 다시 함수가 호출되고 트리거시켜도 이미 애니메이션이 끝났기 때문에 실행되지 않음, 유용하게 쓸 수 있는 방법일 듯
        animator.SetTrigger(open);
        objectWithDuration.enabled = true;
        collider2D.enabled = false;
    }

    // 상자가 열리는 애니메이션이 실행될 때 애니메이션 이벤트로 호출됨
    protected virtual void OpenChest()
    {
        RuntimeManager.PlayOneShot($"event:/SFX/ETC/Chest", transform.position);
        if (isItem)
            ObjectManager.Instance.PopObject(nameof(ItemGameObject), transform.position).GetComponent<ItemGameObject>().Initialize(itemID);
        else    
            ObjectManager.Instance.PopObject(nameof(WeaponGameObject), transform.position).GetComponent<WeaponGameObject>().Initialize(itemID);
    }
}