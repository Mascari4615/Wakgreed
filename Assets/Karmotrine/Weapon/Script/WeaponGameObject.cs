using System.Collections;
using UnityEngine;

public class WeaponGameObject : InteractiveObject
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Weapon weapon;


    private void OnEnable()
    {
        StartCoroutine(Move());
    }

    public void Initialize(int id)
    {
        weapon = DataManager.Instance.WeaponDic[id];
        spriteRenderer.sprite = weapon.sprite;
    }

    public override void Interaction()
    {
        if (Wakgood.Instance.IsSwitching)
        {
            return;
        }

        if (Wakgood.Instance.Weapon[0].id == 0)
        {
            Wakgood.Instance.SwitchWeapon(0, weapon);
            ObjectManager.Instance.PushObject(gameObject);
            return;
        }

        if (Wakgood.Instance.Weapon[1].id == 0)
        {
            Wakgood.Instance.SwitchWeapon(1, weapon);
            ObjectManager.Instance.PushObject(gameObject);
            return;
        }

        Weapon temp = Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber];
        Wakgood.Instance.SwitchWeapon(Wakgood.Instance.CurWeaponNumber, weapon);

        weapon = temp;
        spriteRenderer.sprite = weapon.sprite;
    }

    private IEnumerator Move()
    {
        const float speed = 2f;
        Vector3 waitPosition = transform.position + ((Vector3)Random.insideUnitCircle * 1.5f);
        float temp = 0;

        while (temp < 1)
        {
            yield return null;
            temp += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(transform.position, waitPosition, temp);
        }
    }
}