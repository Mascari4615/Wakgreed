using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : InteractiveObject
{
    // ��޻Ӹ� �ƴ϶� Ư�� ����(����,���,��ƿ,�ó���) �����۸� ������ ���� ���� �͵� ����� �� ����

    [SerializeField] private ItemGrade itemGrade;

    public void Initialize(ItemGrade itemGrade)
    {
        this.itemGrade = itemGrade;
    }

    public void InitializeRandom()
    {
        itemGrade = (ItemGrade)Random.Range(0, 3);
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
        ObjectManager.Instance.PopObject("Item", transform.position).GetComponent<ItemGameObject>().Initialize(itemGrade);
    }
}