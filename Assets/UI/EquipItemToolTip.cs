using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class EquipItemToolTip : MonoBehaviour
{
    private readonly WaitForSeconds ws1 = new (1f);
    private readonly Queue<SpecialThing> toolTipStacks = new();
    private Animator animator;
    [SerializeField] private GameObject toolTip;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI AMGG;
    [SerializeField] private Image image;
    [SerializeField] private ItemVariable lastEquippedItem;
    [SerializeField] private FoodVariable lastEquippedFood;

    private void Awake()
    {
        animator = toolTip.GetComponent<Animator>();
    }

    public void EquipItem()
    {
        toolTipStacks.Enqueue(lastEquippedItem.RuntimeValue);
        if (!toolTip.activeSelf) StartCoroutine(ShowToolTip());
    }

    public void AteFood()
    {
        toolTipStacks.Enqueue(lastEquippedFood.RuntimeValue);
        if (!toolTip.activeSelf) StartCoroutine(ShowToolTip());
    }

    private IEnumerator ShowToolTip()
    {
        toolTip.SetActive(true);

        while (toolTipStacks.Count > 0)
        {
            animator.SetTrigger("ANG");

            SpecialThing item = toolTipStacks.Dequeue();

            if (item is Food)
                AMGG.text = "¿ΩΩƒ º∑√Î!";
            else if (item is Item)
                AMGG.text = "æ∆¿Ã≈€ »πµÊ!";

            nameField.text = item.name;
            image.sprite = item.sprite;
            yield return ws1;
        }
        toolTip.SetActive(false);
    }

    public void StopToolTip()
    {
        toolTip.SetActive(false);
        StopAllCoroutines();
    }
}