using System.Collections;
using FMODUnity;
using UnityEngine;


public class Messi : MonoBehaviour, IEffectGameObject
{
    private SpriteRenderer itemSprite;
    [SerializeField] private float moveSpeed = 1;
    private int percentage = 5;
    private int percentageBonus = 5;

    private void Awake()
    {
        itemSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(FollowWakgood());
    }

    // RuntimeManager.PlayOneShot($"event:/SFX/Item/Wakgi", transform.position);

    /*if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[32]))
        temp += DataManager.Instance.wgItemInven.itemCountDic[32];*/


    private IEnumerator FollowWakgood()
    {
        while (true)
        {
            Vector3 wakgoodPosition = Wakgood.Instance.transform.position;
            transform.position = Vector3.Lerp(transform.position, wakgoodPosition + (transform.position - wakgoodPosition).normalized * 2, Time.deltaTime * moveSpeed);
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

    public void Effect()
    {
        percentageBonus += 5;
    }

    public void Return()
    {
        percentageBonus -= 5;
    }
}
