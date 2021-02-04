using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{

    private Item item;

    public void SetItem(Item _item)
    {
        item = _item;
    }
    
    public void SetItemSlot(Sprite sprite, int count)
    {
        transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().enabled = true;
        transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = count.ToString();
    }
}