using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : InteractiveObject
{
    // 등급뿐만 아니라 특정 컨셉(공격,방어,유틸,시너지) 아이템만 나오는 상자 같은 것도 재밌을 것 같음

    private int itemID;

    [ContextMenu("Linq Test")]
    public void Initialize(ItemGrade itemGrade)
    {
        IEnumerable<int> itemRange = Enumerable.Range(0, 100);

        if (itemGrade == ItemGrade.Common) itemRange = Enumerable.Range(0, 100);
        else if (itemGrade == ItemGrade.Uncommon) itemRange = Enumerable.Range(100, 200);
        else if (itemGrade == ItemGrade.Legendary) itemRange = Enumerable.Range(200, 300);

        var itemList = (from itemID in DataManager.Instance.ItemDic.Keys where itemRange.Contains(itemID) select itemID).ToList();

        itemID = itemList[Random.Range(0, itemList.Count)]; ;
    }

    public override void Interaction()
    {
        // 상자가 열린 이후 다시 함수가 호출되고 트리거시켜도 이미 애니메이션이 끝났기 때문에 실행되지 않음
        // 유용하게 쓸 수 있는 방법일 듯
        GetComponent<Animator>().SetTrigger("OPEN");
    }

    // 상자가 열리는 애니메이션이 실행될 때 애니메이션 이벤트로 호출됨
    private void OpenChest()
    {
        ObjectManager.Instance.GetQueue("Item", transform.position).GetComponent<ItemGameObject>().Initialize(itemID);
    }
}