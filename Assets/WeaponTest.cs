using UnityEngine;

public class WeaponTest : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private WeaponDataBuffer weaponDataBuffer;

    void Start()
    {
        int x = 0;
        int y = 0;

        for (int i = 0; i < weaponDataBuffer.Items.Length; i++)
        {
            GameObject go = Instantiate(weaponPrefab, transform.position + new Vector3(x * 5 -10, y * 5, 0), Quaternion.identity, transform);
            go.GetComponent<WeaponGameObject>().Initialize(i);
            x++;
            if (x >= 5) { x = 0; y++; }
        }
    }

    void Update()
    {
        
    }
}
