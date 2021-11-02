using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAtack", menuName = "Skill/MeleeAtack")]
public class MeleeAtack : Skill
{
    [SerializeField] private float radius = 1;

    public override void Use()
    {
        RaycastHit2D[] hit = Physics2D.CircleCastAll(Wakgood.Instance.AttackPosition.position, radius, Vector2.zero, 0, 1 << 7);
        foreach (var item in hit) item.transform.GetComponent<IHitable>().ReceiveHit(Random.Range(Wakgood.Instance.CurWeapon.minDamage, Wakgood.Instance.CurWeapon.maxDamage));

        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.AttackPosition, true);
        Wakgood.Instance.WeaponPosition.GetChild(0).GetComponent<Animator>()?.SetTrigger("Attack");
        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{Wakgood.Instance.CurWeapon.id}");
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse();
    }
}
