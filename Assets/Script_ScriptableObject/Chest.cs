using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : InteractiveObject
{
    // ��޻Ӹ� �ƴ϶� Ư�� ����(����,���,��ƿ,�ó���) �����۸� ������ ���� ���� �͵� ����� �� ����

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
        // ���ڰ� ���� ���� �ٽ� �Լ��� ȣ��ǰ� Ʈ���Ž��ѵ� �̹� �ִϸ��̼��� ������ ������ ������� ����
        // �����ϰ� �� �� �ִ� ����� ��
        GetComponent<Animator>().SetTrigger("OPEN");
    }

    // ���ڰ� ������ �ִϸ��̼��� ����� �� �ִϸ��̼� �̺�Ʈ�� ȣ���
    private void OpenChest()
    {
        ObjectManager.Instance.GetQueue("Item", transform.position).GetComponent<ItemGameObject>().Initialize(itemID);
    }
}