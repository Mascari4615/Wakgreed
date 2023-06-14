using System.Collections;
using UnityEngine;

public class Messi : MonoBehaviour, IEffectGameObject
{
    [SerializeField] private float moveSpeed = 1;
    private SpriteRenderer itemSprite;
    private readonly int percentage = 5;
    private int percentageBonus = 5;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(FollowWakgood());
    }

    public void Effect()
    {
        percentageBonus += 5;
    }

    public void Return()
    {
        percentageBonus -= 5;
    }

    // RuntimeManager.PlayOneShot($"event:/SFX/Item/Wakgi", transform.position);

    /*if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[32]))
        temp += DataManager.Instance.wgItemInven.itemCountDic[32];*/


    private IEnumerator FollowWakgood()
    {
        while (true)
        {
            spriteRenderer.flipX = Wakgood.Instance.transform.position.x > transform.position.x;
            Vector3 wakgoodPosition = Wakgood.Instance.transform.position;
            transform.position = Vector3.Lerp(transform.position,
                wakgoodPosition + ((transform.position - wakgoodPosition).normalized * 2), Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    public void RoomClear()
    {
        if (Random.Range(0, 100) < percentage + percentageBonus)
        {
            StartCoroutine(GetItem());
        }
    }

    private IEnumerator GetItem()
    {
        Item item = DataManager.Instance.ItemDic[DataManager.Instance.GetRandomItemID()];
        itemSprite.sprite = item.sprite;
        itemSprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        itemSprite.gameObject.SetActive(false);
        DataManager.Instance.wgItemInven.Add(item);
    }
}