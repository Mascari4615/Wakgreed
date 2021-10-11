using FMODUnity;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "MeleeAtack", menuName = "Skill/MeleeAtack")]
public class MeleeAtack : Skill
{
    [SerializeField] private float radius = 1;

    public override void Use(int minDamage, int maxDamage)
    {
        int layerMask = 1 << 7; 
        RaycastHit2D[] hit = Physics2D.CircleCastAll(Wakgood.Instance.attackPosition.position, radius, Vector2.zero, 0, layerMask);
        foreach (var item in hit)
        {
            item.transform.GetComponent<IDamagable>().ReceiveDamage(Random.Range(minDamage, maxDamage + 1));
        }

        ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.attackPosition);
        if (Wakgood.Instance.weaponPosition.GetChild(0).GetComponent<Animator>() != null)
            Wakgood.Instance.weaponPosition.GetChild(0).GetComponent<Animator>().SetTrigger("Attack");
        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{Wakgood.Instance.curWeapon.ID}", Wakgood.Instance.attackPosition.position);
        //string path = $"event:/SFX/Weapon/{Wakgood.Instance.curWeapon.ID}";
        //if ((from event_ in FMODUnity.EventManager.Events where event_.Path.Equals(path) select event_).Count() == 0)
            //RuntimeManager.PlayOneShot("event:/SFX/Weapon/EX_Attack", Wakgood.Instance.attackPosition.position);
        GameManager.Instance.cinemachineImpulseSource.GenerateImpulse();
    }
}
