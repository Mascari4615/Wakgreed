using UnityEngine;

public class WeaponTest : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab;

    private void Start()
    {
        int x = 0, y = 0;

        foreach (var weapon in DataManager.Instance.WeaponDic.Values)
        {
            GameObject go = Instantiate(weaponPrefab, transform.position + new Vector3(x * 5 - 10, y * 5, 0), Quaternion.identity, transform);
            go.GetComponent<WeaponGameObject>().Initialize(weapon.id);
            x++;
            if (x >= 5) { x = 0; y++; }
        }
    }
}