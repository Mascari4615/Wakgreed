using System.Collections;
using UnityEngine;

public class Bat : MonoBehaviour, IEffectGameObject
{
    [SerializeField] private float moveSpeed = 1;
    private bool canWakggiddi = true;
    private readonly float CoolTime = 3f;
    private float curCoolTime = 3f;
    private int stack = 1;

    private void Start()
    {
        curCoolTime = CoolTime;
        StartCoroutine(FollowWakgood());
    }

    public void Effect()
    {
        stack++;
        SetCoolTime();
    }

    public void Return()
    {
        stack--;
        SetCoolTime();
    }

    private IEnumerator FollowWakgood()
    {
        while (true)
        {
            Vector3 wakgoodPosition = Wakgood.Instance.transform.position;
            transform.position = Vector3.Lerp(transform.position,
                wakgoodPosition + ((transform.position - wakgoodPosition).normalized * 2), Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    public void Attack()
    {
        if (canWakggiddi)
        {
            // RuntimeManager.PlayOneShot($"event:/SFX/Item/Wakgi", transform.position);

            ObjectManager.Instance.PopObject("61_Bullet", transform.position).GetComponent<BulletMove>()
                .SetDirection((Vector3)Wakgood.Instance.worldMousePoint - transform.position);

            canWakggiddi = false;
            StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, curCoolTime, value => canWakggiddi = value));
        }
    }

    private void SetCoolTime()
    {
        curCoolTime = CoolTime * (1 - (stack * 10 / 100));
    }
}