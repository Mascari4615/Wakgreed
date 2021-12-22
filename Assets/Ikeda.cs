using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ikeda : ShopKeeper
{
    private SpriteRenderer spriteRenderer;
    private GameObject shadow;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shadow = transform.Find("Shadow").gameObject;

        if (DataManager.Instance.CurGameData.talkedOnceNPC[ID] == false)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0);
            defaultUI.SetActive(false);
            shadow.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        animator.SetBool("BOO", true);
        spriteRenderer.color = new Color(1, 1, 1, 1);

        if (DataManager.Instance.CurGameData.talkedOnceNPC[ID] == false)
        {
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
}