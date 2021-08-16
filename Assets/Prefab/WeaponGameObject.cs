using UnityEngine;

public class WeaponGameObject : InteractiveObject
{
    [SerializeField] private int testWeaponID = 0;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private WeaponDataBuffer WeaponDataBuffer;
    [SerializeField] private Weapon weapon;
    [SerializeField] private MeshRenderer interactionIcon = null;

    private void OnEnable()
    {
        Initialize(testWeaponID);
    }

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
        weapon = WeaponDataBuffer.Items[id];
        spriteRenderer.sprite = weapon.icon;
    }

    public override void Interaction()
    {
        if (!TravellerController.Instance.isSwitching)
        {
            Weapon temp = TravellerController.Instance.curWeapon;
            //Debug.Log(temp.name);
            StartCoroutine(TravellerController.Instance.SwitchWeapon(targetWeapon:weapon));
            //Debug.Log(temp.name);
            weapon = temp;
            //Debug.Log(temp.name);
            spriteRenderer.sprite = weapon.icon;
        }
    }
}
