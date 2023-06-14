using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractiveObject
{
    private static readonly int open = Animator.StringToHash("OPEN");
    [SerializeField] private bool onlyWeapon;

    [SerializeField] private float commonWeight;
    [SerializeField] private float uncommonWeight;
    [SerializeField] private float rareWeight;
    [SerializeField] private float legendWeight;
    private Animator animator;
    private new Collider2D collider2D;
    private bool isItem;

    private bool isOpened;

    protected int itemCount = 1;
    private readonly List<int> itemIDs = new();

    private ObjectWithDuration objectWithDuration;

    protected override void Awake()
    {
        base.Awake();

        objectWithDuration = GetComponent<ObjectWithDuration>();
        animator = GetComponent<Animator>();
        collider2D = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        objectWithDuration.enabled = false;
        collider2D.enabled = true;

        animator.Play("START", 0);

        Probability<Grade> probability = new();
        probability.Add(Grade.Common, commonWeight);
        probability.Add(Grade.Uncommon, uncommonWeight);
        probability.Add(Grade.Rare, rareWeight);
        probability.Add(Grade.Legendary, legendWeight);

        itemIDs.Clear();

        isOpened = false;
        isItem = Random.Range(0, 100) > 20;
        if (onlyWeapon)
        {
            itemIDs.Add(DataManager.Instance.GetRandomWeaponID(probability.Get()));
        }
        else
        {
            if (isItem)
            {
                for (int i = 0; i < itemCount; i++)
                {
                    itemIDs.Add(DataManager.Instance.GetRandomItemID(probability.Get()));
                }
            }
            else
            {
                itemIDs.Add(DataManager.Instance.GetRandomWeaponID(probability.Get()));
            }
        }
    }

    public override void Interaction()
    {
        if (isOpened)
        {
            return;
        }

        isOpened = true;

        // 상자가 열린 이후 다시 함수가 호출되고 트리거시켜도 이미 애니메이션이 끝났기 때문에 실행되지 않음, 유용하게 쓸 수 있는 방법일 듯
        animator.SetTrigger(open);
        objectWithDuration.enabled = true;
        collider2D.enabled = false;
    }

    // 상자가 열리는 애니메이션이 실행될 때 애니메이션 이벤트로 호출됨
    protected virtual void OpenChest()
    {
        RuntimeManager.PlayOneShot("event:/SFX/ETC/Chest", transform.position);

        int randCount = Random.Range(0, 5 + 1);
        randCount = Random.Range(0, 9 + 1);
        for (int i = 0; i < randCount; i++)
        {
            ObjectManager.Instance.PopObject("Goldu", transform);
        }

        if (Random.Range(0, 10) <= 3)
        {
            ObjectManager.Instance.PopObject("HealOrb10", transform);
        }

        if (isItem)
        {
            for (int i = 0; i < itemCount; i++)
            {
                ObjectManager.Instance.PopObject(nameof(ItemGameObject), transform.position)
                    .GetComponent<ItemGameObject>().Initialize(itemIDs[i]);
            }
        }
        else
        {
            ObjectManager.Instance.PopObject(nameof(WeaponGameObject), transform.position)
                .GetComponent<WeaponGameObject>().Initialize(itemIDs[0]);
        }
    }
}