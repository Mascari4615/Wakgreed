using System.Collections;
using FMODUnity;
using UnityEngine;

public class MiniWakpago : MonoBehaviour, IEffectGameObject
{
    [SerializeField] GameObject Bullet;
    private BulletMove bullet;
    [SerializeField] private float moveSpeed = 1;
    private bool canWakggiddi = true;
    private const float CoolTime = 3f;
    private int damage = 5;

    private void Awake()
    {
        bullet = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<BulletMove>();
        bullet.gameObject.SetActive(false);
        bullet.gameObject.GetComponent<DamagingObject>().damage = damage;
    }

    private void Start()
    {
        StartCoroutine(FollowWakgood());
    }

    private IEnumerator FollowWakgood()
    {
        WaitForSeconds ws01 = new(0.1f);
        while (true)
        {
            if (canWakggiddi)
            {
                if (GameManager.Instance.enemyRunTimeSet.Items.Count > 0)
                {
                    Vector3 originalDistance = transform.position - Wakgood.Instance.transform.position;
                    Transform mob = GetNearestMob();

                    // RuntimeManager.PlayOneShot($"event:/SFX/Item/Wakgi", transform.position);

                    bullet.transform.position = transform.position;
                    bullet.SetDirection(mob .position - transform.position);
                    bullet.gameObject.SetActive(true);

                    canWakggiddi = false;
                    StartCoroutine(TtmdaclExtension.ChangeWithDelay(true, CoolTime, (value) => canWakggiddi = value));
                }
            }

            Vector3 position = transform.position;
            Vector3 wakgoodPosition = Wakgood.Instance.transform.position;
            position = Vector3.Lerp(position, wakgoodPosition + (position - wakgoodPosition).normalized * 2, Time.deltaTime * moveSpeed);
            transform.position = position;

            yield return null;
        }
    }

    private Transform GetNearestMob()
    {
        Transform target = null;
        float targetDist = 100;
        float currentDist;

        foreach (GameObject monster in GameManager.Instance.enemyRunTimeSet.Items)
        {
            currentDist = Vector2.Distance(transform.position, monster.transform.position);
            if (currentDist > targetDist) continue;

            target = monster.transform;
            targetDist = currentDist;
        }

        return target;
    }

    public void Effect()
    {
        damage += 5;
        SetDamage();
    }

    public void Return()
    {
        damage -= 5;
        SetDamage();
    }

    private void SetDamage()
    {
        if (bullet.gameObject != null)
        {
            bullet.gameObject.GetComponent<DamagingObject>().damage = damage;
        }
    }
}
