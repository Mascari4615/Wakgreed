using System.Collections;
using UnityEngine;

public class Bat : MonoBehaviour, IEffectGameObject
{
    [SerializeField] private float moveSpeed = 1;
    private bool canWakggiddi = true;
    private float CoolTime = 3f;

    private void Start()
    {
        StartCoroutine(FollowWakgood());
    }

    private IEnumerator FollowWakgood()
    {
        while (true)
        {
            Vector3 wakgoodPosition = Wakgood.Instance.transform.position;
            transform.position = Vector3.Lerp(transform.position, wakgoodPosition + (transform.position - wakgoodPosition).normalized * 2, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    public void Attack()
    {
        if (canWakggiddi)
        {
            // RuntimeManager.PlayOneShot($"event:/SFX/Item/Wakgi", transform.position);

            ObjectManager.Instance.PopObject("61_Bullet", Wakgood.Instance.transform.position).GetComponent<BulletMove>().SetDirection((Vector3)Wakgood.Instance.worldMousePoint - Wakgood.Instance.transform.position);

            canWakggiddi = false;
            StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, CoolTime, (value) => canWakggiddi = value));
        }
    }

    public void Effect()
    {
        Debug.Log("¹ÚÁã Effect ¹ö±×");
    }

    public void Return()
    {
        Debug.Log("¹ÚÁã Return ¹ö±×");
    }
}