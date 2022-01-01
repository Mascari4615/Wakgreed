using FMODUnity;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "RisingMoonQSkill", menuName = "Skill/RisingMoonQSkill")]
public class RisingMoonQSkill : Skill
{
    private Animator animator;

    public override void Use(Weapon weapon)
    {
        Wakgood.Instance.StartCoroutine(_Use(weapon));
    }

    private  IEnumerator _Use(Weapon weapon)
    {
        if (Wakgood.Instance.WeaponPosition.GetChild(0).TryGetComponent(out animator))
            animator.SetTrigger("Attack");

        RuntimeManager.PlayOneShot($"event:/SFX/Weapon/{weapon.id}");
        GameManager.Instance.CinemachineImpulseSource.GenerateImpulse();

        if (ObjectManager.Instance.CheckPool(resource.name) == false)
            ObjectManager.Instance.AddPool(resource);

        for (int k = 0; k < 15; k++)
        {
            ObjectManager.Instance.PopObject(resource.name, Wakgood.Instance.transform.position + (Vector3)Random.insideUnitCircle * 12f,
                new Vector3(0, 0, Random.Range(0f, 180f)));
            RuntimeManager.PlayOneShot($"event:/SFX/Weapon/2");
            yield return new WaitForSeconds(.05f);
        }
    }
}
