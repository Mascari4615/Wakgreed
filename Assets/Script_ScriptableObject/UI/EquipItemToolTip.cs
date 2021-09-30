using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipItemToolTip : MonoBehaviour
{
    private Queue<Item> toolTipStacks = new();
    [SerializeField] private GameObject toolTip;
    [SerializeField] private Text nameField;
    [SerializeField] private Text simpleDescriptionField;
    [SerializeField] private Image image;
    [SerializeField] private ItemVariable LastEquippedItem;
    private bool isShowing = false;

    public void EquipItem()
    {
        if (LastEquippedItem) Debug.Log("1");
        if (LastEquippedItem.RuntimeValue) Debug.Log("2");
        if (LastEquippedItem.RuntimeValue.itemGrade == ItemGrade.Material) Debug.Log("3");

        if (LastEquippedItem.RuntimeValue.itemGrade == ItemGrade.Material) { Debug.Log("ASD"); return; }
        toolTipStacks.Enqueue(LastEquippedItem.RuntimeValue);
        if (!isShowing) StartCoroutine(ShowToolTip());
    }

    private IEnumerator ShowToolTip()
    {
        Item item;
        WaitForSeconds ws2 = new(2f);

        isShowing = true;
        toolTip.SetActive(true);
        while (toolTipStacks.Count > 0)
        {
            item = toolTipStacks.Dequeue();
            nameField.text = item.name;
            simpleDescriptionField.text = item.simpleDescription;
            image.sprite = item.sprite;
            yield return ws2;
        }
        toolTip.SetActive(false);
        isShowing = false;
    }
}