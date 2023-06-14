using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ikeda : NPC
{
    [SerializeField] protected FoodInventoryUI inventoryUI;
    [SerializeField] protected FoodDataBuffer foodDataBuffer;
    [SerializeField] protected IntVariable goldu;
    private GameObject fakeChest;
    private GameObject shadow;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();
        shadow = transform.Find("Shadow").gameObject;
        fakeChest = transform.Find("FakeChest").gameObject;

        if (DataManager.Instance.CurGameData.talkedOnceNPC[ID] == false)
        {
            fakeChest.SetActive(true);
            spriteRenderer.color = new Color(1, 1, 1, 0);
            defaultUI.SetActive(false);
            shadow.SetActive(false);
        }
        else
        {
            fakeChest.SetActive(false);
        }

        ResetInven();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (!other.CompareTag("Player"))
        {
            return;
        }

        animator.SetBool("BOO", true);
        spriteRenderer.color = new Color(1, 1, 1, 1);

        if (DataManager.Instance.CurGameData.talkedOnceNPC[ID] == false)
        {
            if (DataManager.Instance.CurGameData.rescuedNPC[ID] == false)
            {
                DataManager.Instance.CurGameData.rescuedNPC[ID] = true;
            }

            fakeChest.SetActive(false);
            defaultUI.SetActive(true);
            shadow.SetActive(true);
            Interaction();
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        animator.SetBool("BOO", false);
    }

    public void ResetInven()
    {
        List<Food> temp = foodDataBuffer.items.ToList();
        inventoryUI.NpcInventory.Clear();
        for (int i = 0; i < 5; i++)
        {
            int random = Random.Range(0, temp.Count);
            inventoryUI.NpcInventory.Add(temp[random]);
            temp.RemoveAt(random);
        }
    }

    public virtual void BuyFood(Slot slot)
    {
        if (goldu.RuntimeValue >= (slot.SpecialThing as Food).price)
        {
            goldu.RuntimeValue -= (slot.SpecialThing as Food).price;

            slot.gameObject.SetActive(false);
            inventoryUI.NpcInventory.Remove(slot.SpecialThing as Food);
            DataManager.Instance.wgFoodInven.Add(slot.SpecialThing as Food);
            RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position)
                .GetComponent<AnimatedText>().SetText("골두 부족!", Color.yellow);
            RuntimeManager.PlayOneShot("event:/SFX/UI/No", transform.position);
        }
    }
}