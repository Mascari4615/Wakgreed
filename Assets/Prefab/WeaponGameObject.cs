using UnityEngine;

public class WeaponGameObject : InteractiveObject
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Weapon weapon;
    [SerializeField] private MeshRenderer interactionIcon = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            interactionIcon.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            interactionIcon.enabled = false;
        }
    }

    public void Initialize(int id)
    {
        weapon = DataManager.Instance.WeaponDic[id];
        spriteRenderer.sprite = weapon.icon;
    }

    public override void Interaction()
    {
        if (!TravellerController.Instance.isSwitching)
        {
            Weapon temp = TravellerController.Instance.curWeapon;
            TravellerController.Instance.SwitchWeapon(targetWeapon:weapon);
            weapon = temp;
            spriteRenderer.sprite = weapon.icon;
        }
    }
}
