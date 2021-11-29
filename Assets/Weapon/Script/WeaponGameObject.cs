using UnityEngine;

public class WeaponGameObject : InteractiveObject
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Weapon weapon;
    [SerializeField] private MeshRenderer interactionIcon;

    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) interactionIcon.enabled = true; }
    private void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) interactionIcon.enabled = false; }

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

        Weapon temp = Wakgood.Instance.CurWeapon;
        Wakgood.Instance.SwitchWeapon(Wakgood.Instance.CurWeaponNumber, weapon);
        weapon = temp;
        spriteRenderer.sprite = weapon.sprite;
    }
}
