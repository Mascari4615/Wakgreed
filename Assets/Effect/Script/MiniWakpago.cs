using System.Collections;
using FMODUnity;
using UnityEngine;

public class MiniWakpago : MonoBehaviour, IEffectGameObject
{
    [SerializeField] GameObject Bullet;
    private BulletMove[] bullet;
    [SerializeField] private float moveSpeed = 1;
    private bool canWakggiddi = true;
    private float CoolTime = 3f;
    private int damage = 5;

    private void Awake()
    {
        bullet = new BulletMove[10];

        for (int i = 0; i < 10; i++)
        {
            bullet[i] = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<BulletMove>();
            bullet[i].gameObject.SetActive(false);
            bullet[i].gameObject.GetComponent<DamagingObject>().damage = damage;
        }

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
                    Transform mob = GetNearestMob();
                    if (mob == null) continue;
                    // RuntimeManager.PlayOneShot($"event:/SFX/Item/Wakgi", transform.position);

                    int temp = 1;

                    if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[32]))
                        temp += DataManager.Instance.wgItemInven.itemCountDic[32];

                    for (int i = 0; i < temp; i++)
                    {
                        bullet[i].transform.position = transform.position;
                        bullet[i].SetDirection(mob.position - transform.position);
                        bullet[i].gameObject.SetActive(true);

                        if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[28]))
                            CoolTime = 3 * (1 - DataManager.Instance.wgItemInven.itemCountDic[28] * 20 / 100);

                        yield return new WaitForSeconds(0.2f);
                    }

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
        if (bullet != null)
        {
            for (int i = 0; i < 10; i++)
            {
                bullet[i].gameObject.GetComponent<DamagingObject>().damage = damage;
            }
        }
    }
}