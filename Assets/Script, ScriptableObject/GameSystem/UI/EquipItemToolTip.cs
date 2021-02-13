using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class EquipItemToolTip : MonoBehaviour
{
    private Queue<Item> toolTipStacks = new Queue<Item>();
    [SerializeField] private GameObject toolTip;
    [SerializeField] private Text nameField;
    [SerializeField] private Text simpleDescriptionField;
    [SerializeField] private Image image;
    [SerializeField] private Inventory Inventory;
    private bool isShowing = false;

    public void EquipItem()
    {
        toolTipStacks.Enqueue(Inventory.Items[Inventory.Items.Count - 1]);
        if (!isShowing) StartCoroutine(ShowToolTip());
    }

    private IEnumerator ShowToolTip()
    {
        isShowing = true;
        while (toolTipStacks.Count > 0)
        {
            Item item = toolTipStacks.Dequeue();
            nameField.text = item.name;
            simpleDescriptionField.text = item.simpleDescription;
            image.sprite = item.sprite;
            toolTip.SetActive(true);
            yield return new WaitForSeconds(3f);
        }
        toolTip.SetActive(false);
        isShowing = false;
    }
}