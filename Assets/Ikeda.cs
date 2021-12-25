using UnityEngine;

public class Ikeda : Chef
{
    private SpriteRenderer spriteRenderer;
    private GameObject shadow;
    private GameObject fakeChest;

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
            if (DataManager.Instance.CurGameData.rescuedNPC[ID] == false)
                DataManager.Instance.CurGameData.rescuedNPC[ID] = true;

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
}