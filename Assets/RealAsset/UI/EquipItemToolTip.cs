using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipItemToolTip : MonoBehaviour
{
    private Queue<Item> toolTipStacks = new();
    [SerializeField] private GameObject toolTip;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI simpleDescriptionField;
    [SerializeField] private Image image;
    [SerializeField] private ItemVariable LastEquippedItem;

    public void EquipItem()
    {
        if (LastEquippedItem.RuntimeValue.itemGrade == ItemGrade.Material) return;
        toolTipStacks.Enqueue(LastEquippedItem.RuntimeValue);
        if (!toolTip.activeSelf) StartCoroutine(ShowToolTip());
    }

    private IEnumerator ShowToolTip()
    {
        Item item;
        WaitForSeconds ws2 = new(2f);

        toolTip.SetActive(true);
        while (toolTipStacks.Count > 0)
        {
            item = toolTipStacks.Dequeue();
            nameField.text = item.name;
            simpleDescriptionField.text = item.description;
            image.sprite = item.sprite;
            yield return ws2;
        }
        toolTip.SetActive(false);
    }

    public void StopToolTip()
    {
        toolTip.SetActive(false);
        StopAllCoroutines();
    }
}